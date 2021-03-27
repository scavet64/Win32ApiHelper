using Win32ApiCore.Win32Api;
using Win32ApiCore.WindowsInteropEnums;
using Win32ApiCore.WindowsInteropStructs;
using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;

namespace Win32ApiCore
{
    /// <summary>
    /// Provide a context to impersonate operations.
    /// </summary>
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public class Impersonator : IDisposable
    {
        private readonly Win32ApiService win32ApiService;

        private WindowsImpersonationContext impersonationContext;

        /// <summary>
        /// Initialize a new instance of the Impersonator class with the specified user name, pass, and domain.
        /// </summary>
        /// <param name="userName">The user name associated with the impersonation.</param>
        /// <param name="pass">The pass for the user name associated with the impersonation.</param>
        /// <param name="domain">The domain associated with the impersonation.</param>
        /// <exception cref="ArgumentException">If the logon operation failed.</exception>
        public Impersonator(string userName, string pass, string domain)
        {
            SetupImpersonation(userName, pass, domain);
        }

        /// <summary>
        /// Initialize a new instance of the Impersonator class using the currently logged on user.
        /// This is useful when we need to execute code as the currently logged in user from a service
        /// or when running the application as a differnt user.
        /// </summary>
        /// <param name="win32ApiService">The windows 32 API service</param>
        public Impersonator(Win32ApiService win32ApiService)
        {
            this.win32ApiService = win32ApiService;
            SetupImpersonation();
        }

        /// <summary>
        /// Default form of impersonation. This method will impersonate the currently logged in user.
        /// </summary>
        private void SetupImpersonation()
        {
            var hUserToken = IntPtr.Zero;

            try
            {
                if (this.win32ApiService.GetSessionUserToken(ref hUserToken))
                {
                    var id = new WindowsIdentity(hUserToken);
                    impersonationContext = id.Impersonate();
                }
                else
                {
                    throw new Win32ApiException("GetCurrentLoggedInUsername: GetSessionUserToken failed.");
                }
            }
            finally
            {
                WindowsInteropApi.CloseHandle(hUserToken);
            }
        }

        /// <summary>
        /// Impersonate the specified user
        /// </summary>
        /// <param name="userName">The windows username</param>
        /// <param name="pass">The pass for that user</param>
        /// <param name="domain">The domain for that user</param>
        private void SetupImpersonation(string userName, string pass, string domain)
        {
            WindowsIdentity tempWindowsIdentity;
            IntPtr userAccountToken = IntPtr.Zero;
            IntPtr tokenDuplicate = IntPtr.Zero;

            try
            {
                if (WindowsInteropApi.LogonUserA(
                    userName, 
                    domain, 
                    pass, 
                    LogonTypes.Interactive,
                    LogonProvider.LOGON32_PROVIDER_DEFAULT,
                    ref userAccountToken) == 0)
                {
                    var primaryToken = GetPrimaryToken(userAccountToken);

                    if (WindowsInteropApi.DuplicateToken(userAccountToken, 2, ref tokenDuplicate) != 0)
                    {
                        tempWindowsIdentity = new WindowsIdentity(tokenDuplicate);
                        impersonationContext = tempWindowsIdentity.Impersonate();
                        if (impersonationContext != null)
                        {
                            return;
                        }
                    }
                }
            }
            finally
            {
                // Close the token handle.
                if (userAccountToken != IntPtr.Zero)
                {
                    WindowsInteropApi.CloseHandle(userAccountToken);
                }
                if (tokenDuplicate != IntPtr.Zero)
                {
                    WindowsInteropApi.CloseHandle(tokenDuplicate);
                }
            }

            throw new ArgumentException($"Logon operation failed[{Marshal.GetLastWin32Error()}] for userName {userName}.");
        }

        private IntPtr GetPrimaryToken(IntPtr token)
        {
            var primaryToken = IntPtr.Zero;

            var sa = new SECURITY_ATTRIBUTES();
            sa.Length = Marshal.SizeOf(sa);

            if (!WindowsInteropApi.DuplicateTokenEx(
                token,
                WindowsInteropConstants.TokenAllAccess,
                ref sa,
                (int)SECURITY_IMPERSONATION_LEVEL.SecurityImpersonation,
                (int)TOKEN_TYPE.TokenPrimary,
                ref primaryToken))
            {
                throw new Win32ApiException($"DuplicateTokenEx failed with code: {Marshal.GetLastWin32Error()}");
            }

            WindowsInteropApi.CloseHandle(token);

            return primaryToken;
        }

        /// <summary>
        /// Reverts the user context to the Windows user represented by the WindowsImpersonationContext.
        /// </summary>
        public void UndoImpersonation()
        {
            impersonationContext.Undo();
        }

        /// <summary>
        /// Releases all resources used by <see cref="Impersonator"/> :
        /// - Reverts the user context to the Windows user represented by this object : <see cref="System.Security.Principal.WindowsImpersonationContext"/>.Undo().
        /// - Dispose the <see cref="System.Security.Principal.WindowsImpersonationContext"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose of the object
        /// </summary>
        /// <param name="disposing">Controls if this should run the desposing procedure again</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (impersonationContext != null)
                {
                    UndoImpersonation();
                    impersonationContext.Dispose();
                    impersonationContext = null;
                }
            }
        }
    }
}
