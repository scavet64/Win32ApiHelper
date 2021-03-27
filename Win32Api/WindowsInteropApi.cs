using Win32ApiCore.WindowsInteropEnums;
using Win32ApiCore.WindowsInteropStructs;
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Win32ApiCore.Win32Api
{
    /// <summary>
    /// Static class containing the imported extern methods from various windows apis
    /// </summary>
    public static class WindowsInteropApi
    {
        /// <summary>
        /// Closes an open object handle.
        /// </summary>
        /// <param name="hSnapshot">A valid handle to an open object.</param>
        /// <returns>If the function succeeds, the return value is true.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(
            IntPtr hSnapshot);

        /// <summary>
        /// Retrieves the session identifier of the console session. The console session is the session that is 
        /// currently attached to the physical console. Note that it is not necessary that Remote Desktop Services 
        /// be running for this function to succeed.
        /// </summary>
        /// <returns>The session identifier of the session that is attached to the physical console.</returns>
        [DllImport("kernel32.dll")]
        public static extern uint WTSGetActiveConsoleSessionId();

        /// <summary>
        /// Obtains the primary access token of the logged-on user specified by the session ID. 
        /// To call this function successfully, the calling application must be running within the context of 
        /// the LocalSystem account and have the SE_TCB_NAME privilege.
        /// </summary>
        /// <param name="sessionId">The active session id</param>
        /// <param name="phToken">
        ///     If the function succeeds, receives a pointer to the token handle for the logged-on user.
        /// </param>
        /// <returns>
        ///     If the function succeeds, the return value is a nonzero value, and the phToken parameter
        ///     points to the primary token of the user.
        /// </returns>
        [DllImport("Wtsapi32.dll", SetLastError = true)]
        public static extern uint WTSQueryUserToken(
            uint sessionId, 
            ref IntPtr phToken);

        /// <summary>
        /// Retrieves a list of sessions on a Remote Desktop Session Host (RD Session Host) server.
        /// </summary>
        /// <param name="hServer">A handle to the RD Session Host server.</param>
        /// <param name="reserved">This parameter is reserved. It must be zero.</param>
        /// <param name="version">The version of the enumeration request. This parameter must be 1.</param>
        /// <param name="ppSessionInfo">
        ///     A pointer to an array of WTS_SESSION_INFO structures that represent the retrieved sessions. 
        ///     To free the returned buffer, call the WTSFreeMemory function.
        /// </param>
        /// <param name="pCount">
        ///     A pointer to the number of WTS_SESSION_INFO structures returned in the ppSessionInfo parameter.
        /// </param>
        /// <returns>Returns zero if this function fails. If this function succeeds, a nonzero value is returned.</returns>
        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern int WTSEnumerateSessions(
            IntPtr hServer,
            int reserved,
            int version,
            ref IntPtr ppSessionInfo,
            ref int pCount);

        /// <summary>
        ///     Creates a new process and its primary thread. The new process runs in the security context of the 
        ///     user represented by the specified token. See more information at https://docs.microsoft.com/en-us/windows/win32/api/processthreadsapi/nf-processthreadsapi-createprocessasusera
        /// </summary>
        /// <param name="hToken">
        ///     A handle to the primary token that represents a user. The handle must have the
        ///     TOKEN_QUERY, TOKEN_DUPLICATE, and TOKEN_ASSIGN_PRIMARY access rights.
        /// </param>
        /// <param name="lpApplicationName">
        ///     The name of the module to be executed.
        /// </param>
        /// <param name="lpCommandLine">
        ///     The command line to be executed. The maximum length of this string is 32K characters. If 
        ///     lpApplicationName is NULL, the module name portion of lpCommandLine is limited to MAX_PATH characters.
        /// </param>
        /// <param name="lpProcessAttributes">
        ///     A pointer to a SECURITY_ATTRIBUTES structure that specifies a security descriptor for the new process
        ///     object and determines whether child processes can inherit the returned handle to the process.
        /// </param>
        /// <param name="lpThreadAttributes">
        ///     A pointer to a SECURITY_ATTRIBUTES structure that specifies a security descriptor for the new thread 
        ///     object and determines whether child processes can inherit the returned handle to the thread.
        /// </param>
        /// <param name="bInheritHandle">
        ///     If this parameter is TRUE, each inheritable handle in the calling process is inherited by the new 
        ///     process. If the parameter is FALSE, the handles are not inherited. 
        /// </param>
        /// <param name="dwCreationFlags">
        ///     The flags that control the priority class and the creation of the process.
        /// </param>
        /// <param name="lpEnvironment">
        ///     A pointer to an environment block for the new process. If this parameter is NULL, the new process 
        ///     uses the environment of the calling process.
        /// </param>
        /// <param name="lpCurrentDirectory">
        ///     The full path to the current directory for the process. The string can also specify a UNC path.
        /// </param>
        /// <param name="lpStartupInfo">
        ///     A pointer to a STARTUPINFO or STARTUPINFOEX structure.The user must have full access to both the 
        ///     specified window station and desktop. If you want the process to be interactive, specify winsta0\default.
        /// </param>
        /// <param name="lpProcessInformation">
        ///     A pointer to a PROCESS_INFORMATION structure that receives identification information about the 
        ///     new process.
        /// </param>
        /// <returns>If the function succeeds, the return value is true.</returns>
        [DllImport("advapi32.dll", EntryPoint = "CreateProcessAsUser", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool CreateProcessAsUser(
            IntPtr hToken, 
            string lpApplicationName, 
            string lpCommandLine, 
            ref SECURITY_ATTRIBUTES lpProcessAttributes,
            ref SECURITY_ATTRIBUTES lpThreadAttributes, 
            bool bInheritHandle, 
            int dwCreationFlags, 
            IntPtr lpEnvironment,
            string lpCurrentDirectory, 
            ref STARTUPINFO lpStartupInfo, 
            out PROCESS_INFORMATION lpProcessInformation);

        /// <summary>
        /// The DuplicateTokenEx function creates a new access token that duplicates an existing token. 
        /// This function can create either a primary token or an impersonation token.
        /// </summary>
        /// <param name="existingTokenHandle">
        ///     A handle to an access token opened with TOKEN_DUPLICATE access.
        /// </param>
        /// <param name="dwDesiredAccess">
        ///     Specifies the requested access rights for the new token. The 
        ///     DuplicateTokenEx function compares the requested access rights with the existing token's 
        ///     discretionary access control list (DACL) to determine which rights are granted or denied. 
        ///     To request the same access rights as the existing token, specify zero. To request all access 
        ///     rights that are valid for the caller, specify MAXIMUM_ALLOWED.
        /// </param>
        /// <param name="lpThreadAttributes">
        ///     A pointer to a SECURITY_ATTRIBUTES structure that specifies a 
        ///     security descriptor for the new token and determines whether child processes can inherit the token.
        ///     If lpTokenAttributes is NULL, the token gets a default security descriptor and the handle cannot 
        ///     be inherited. If the security descriptor contains a system access control list (SACL), the token 
        ///     gets ACCESS_SYSTEM_SECURITY access right, even if it was not requested in dwDesiredAccess. 
        ///     To set the owner in the security descriptor for the new token, the caller's process token must have 
        ///     the SE_RESTORE_NAME privilege set.
        /// </param>
        /// <param name="impersonationLevel">
        ///     Specifies a value from the SECURITY_IMPERSONATION_LEVEL enumeration
        ///     that indicates the impersonation level of the new token.
        /// </param>
        /// <param name="tokenType">
        ///     Specifies one of the following values from the TOKEN_TYPE enumeration.
        /// </param>
        /// <param name="duplicateTokenHandle">
        ///     A pointer to a HANDLE variable that receives the new token.
        /// </param>
        /// <returns>True if successful</returns>
        [DllImport("advapi32.dll", EntryPoint = "DuplicateTokenEx", SetLastError = true)]
        public static extern bool DuplicateTokenEx(
            IntPtr existingTokenHandle, 
            uint dwDesiredAccess,
            ref SECURITY_ATTRIBUTES lpThreadAttributes, 
            int impersonationLevel,
            int tokenType, 
            ref IntPtr duplicateTokenHandle);

        /// <summary>
        /// Opens an existing local process object.
        /// </summary>
        /// <param name="dwDesiredAccess">
        ///     The access to the process object. This access right is checked against the security descriptor for 
        ///     the process. This parameter can be one or more of the process access rights.
        /// </param>
        /// <param name="bInheritHandle">
        ///     If this value is TRUE, processes created by this process will inherit the handle. Otherwise, the 
        ///     processes do not inherit this handle.
        /// </param>
        /// <param name="dwProcessId">
        ///     The identifier of the local process to be opened.
        /// </param>
        /// <returns>If the function succeeds, the return value is an open handle to the specified process.</returns>
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(
            uint dwDesiredAccess,
            bool bInheritHandle, 
            uint dwProcessId);

        /// <summary>
        /// The OpenProcessToken function opens the access token associated with a process.
        /// </summary>
        /// <param name="processHandle">A handle to the process whose access token is opened. The process must have the 
        /// PROCESS_QUERY_INFORMATION access permission.</param>
        /// <param name="desiredAccess">Specifies an access mask that specifies the requested types of access to the 
        /// access token. These requested access types are compared with the discretionary access control list (DACL) 
        /// of the token to determine which accesses are granted or denied.</param>
        /// <param name="tokenHandle">A pointer to a handle that identifies the newly opened access token when the 
        /// function returns.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("advapi32", SetLastError = true), SuppressUnmanagedCodeSecurityAttribute]
        public static extern bool OpenProcessToken(
            IntPtr processHandle,
            int desiredAccess, 
            ref IntPtr tokenHandle);

        /// <summary>
        /// The LogonUser function attempts to log a user on to the local computer. The local computer is the 
        /// computer from which LogonUser was called. You cannot use LogonUser to log on to a remote computer.
        /// You specify the user with a user name and domain and authenticate the user with a plaintext pass.
        /// If the function succeeds, you receive a handle to a token that represents the logged-on user. 
        /// You can then use this token handle to impersonate the specified user or, in most cases, to create 
        /// a process that runs in the context of the specified user.
        /// </summary>
        /// <param name="lpszUserName">This is the name of the user account to log on to. If you use the user 
        /// principal name (UPN) format, User@DNSDomainName, the lpszDomain parameter must be NULL.</param>
        /// <param name="lpszDomain">Specifies the name of the domain or server whose account database contains
        /// the lpszUsername account. If this parameter is NULL, the user name must be specified in UPN format. If this parameter is ".", the function validates the account by using only the local account database.</param>
        /// <param name="lpszPass">A pointer to a null-terminated string that specifies the plaintext 
        /// pass for the user account specified by lpszUsername. When you have finished using the pass,
        /// clear the pass from memory by calling the SecureZeroMemory function. For more information about
        /// protecting passes, see Handling passes.</param>
        /// <param name="dwLogonType">The type of logon operation to perform.</param>
        /// <param name="dwLogonProvider">Specifies the logon provider. </param>
        /// <param name="phToken">A pointer to a handle variable that receives a handle to a token that 
        /// represents the specified user.</param>
        /// <returns>If the function succeeds, the function returns nonzero.</returns>
        [DllImport("advapi32.dll")]
        public static extern int LogonUserA(
            string lpszUserName,
            string lpszDomain,
            string lpszPass,
            LogonTypes dwLogonType,
            LogonProvider dwLogonProvider,
            ref IntPtr phToken);

        /// <summary>
        /// The DuplicateToken function creates a new access token that duplicates one already in existence.
        /// </summary>
        /// <param name="hExistingToken">A handle to an access token opened with TOKEN_DUPLICATE access.</param>
        /// <param name="dwDesiredAccess">Specifies a SECURITY_IMPERSONATION_LEVEL enumerated type that 
        /// supplies the impersonation level of the new token.</param>
        /// <param name="phNewToken">A pointer to a variable that receives a handle to the duplicate token.
        /// This handle has TOKEN_IMPERSONATE and TOKEN_QUERY access to the new token.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int DuplicateToken(
            IntPtr hExistingToken,
            int dwDesiredAccess,
            ref IntPtr phNewToken);

        /// <summary>
        /// Imported method to place a window on the desktop in a certain z order
        /// </summary>
        /// <param name="hWnd">handle of window</param>
        /// <param name="hWndInsertAfter">where in the z order to place the window (-1 is at front, 1 is at back)</param>
        /// <param name="x">left side coordinate</param>
        /// <param name="y">top side coordinate</param>
        /// <param name="cx">width of application</param>
        /// <param name="cy">height of application</param>
        /// <param name="wFlags">flags dictating the placement of the application</param>
        /// <returns>pointer indicating success of move</returns>
        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(
            IntPtr hWnd,
            IntPtr hWndInsertAfter,
            int x,
            int y,
            int cx,
            int cy,
            uint wFlags);

        /// <summary>
        /// Retrieves a handle to the top-level window whose class name and window name match the specified strings. 
        /// This function does not search child windows. This function does not perform a case-sensitive search.
        /// </summary>
        /// <param name="className">The window class name</param>
        /// <param name="windowText">The window name</param>
        /// <returns>If the function succeeds, the return value is a handle to the window that has the specified
        /// class name and window name.</returns>
        [DllImport("user32.dll")]
        public static extern int FindWindow(string className, string windowText);

        /// <summary>
        /// Retrieves a handle to a window whose class name and window name match the specified strings. 
        /// The function searches child windows, beginning with the one following the specified child window. 
        /// This function does not perform a case-sensitive search.
        /// </summary>
        /// <param name="parentHandle">A handle to the parent window whose child windows are to be searched.</param>
        /// <param name="childAfter">A handle to a child window.</param>
        /// <param name="lclassName">The class name or a class atom created by a previous call to the RegisterClass or RegisterClassEx function.</param>
        /// <param name="windowTitle">The window name (the window's title). If this parameter is NULL, all window names match.</param>
        /// <returns>If the function succeeds, the return value is a handle to the window that has the specified class and window names.</returns>
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int FindWindowEx(int parentHandle, IntPtr childAfter, string lclassName, string windowTitle);

        /// <summary>
        /// Sets the specified window's show state.
        /// </summary>
        /// <param name="hwnd">A handle to the window.</param>
        /// <param name="command">Controls how the window is to be shown.</param>
        /// <returns>If the window was previously visible, the return value is nonzero. If the window was 
        /// previously hidden, the return value is zero.</returns>
        [DllImport("user32.dll")]
        public static extern int ShowWindow(int hwnd, int command);

        /// <summary>
        /// Enables an application to inform the system that it is in use, thereby preventing the system from entering 
        /// sleep or turning off the display while the application is running.
        /// </summary>
        /// <param name="esFlags">The thread's execution requirements</param>
        /// <returns>If the function succeeds, the return value is the previous thread execution state.</returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern ExecutionState SetThreadExecutionState(ExecutionState esFlags);
    }
}
