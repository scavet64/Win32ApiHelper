using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Win32ApiCore.Win32Api;
using Win32ApiCore.WindowsInteropEnums;
using Win32ApiCore.WindowsInteropStructs;
using System.Security.Principal;

namespace Win32ApiCore
{
    /// <summary>
    /// Service that is responsible for calling the Win32Api.
    /// </summary>
    public class Win32ApiService
    {
        private readonly IntPtr topMostPtr = new IntPtr(-1);

        /// <summary>
        /// Constructor for the Win32ApiService.
        /// </summary>
        public Win32ApiService() {}

        /// <summary>
        /// Launches the given application with full admin rights, and in addition bypasses the UAC prompt
        /// </summary>
        /// <param name="applicationName">The name of the application to launch</param>
        /// <param name="procInfo">Process information regarding the launched application that gets returned to the caller</param>
        /// <returns>True if the operation was successful</returns>
        public bool StartProcessAndBypassUAC(string applicationName, out PROCESS_INFORMATION procInfo)
        {
            uint winlogonPid = 0;
            IntPtr hUserTokenDup = IntPtr.Zero, hPToken = IntPtr.Zero, hProcess = IntPtr.Zero;
            procInfo = new PROCESS_INFORMATION();

            // obtain the currently active session id; every logged on user in the system has a unique session id
            uint dwSessionId = WindowsInteropApi.WTSGetActiveConsoleSessionId();

            // obtain the process id of the winlogon process that is running within the currently active session
            Process[] processes = Process.GetProcessesByName("winlogon");
            foreach (Process p in processes)
            {
                if ((uint)p.SessionId == dwSessionId)
                {
                    winlogonPid = (uint)p.Id;
                }
            }

            // obtain a handle to the winlogon process
            hProcess = WindowsInteropApi.OpenProcess(WindowsInteropConstants.MaximumAllowed, false, winlogonPid);

            // obtain a handle to the access token of the winlogon process
            if (!WindowsInteropApi.OpenProcessToken(hProcess, WindowsInteropConstants.TokenDuplicate, ref hPToken))
            {
                WindowsInteropApi.CloseHandle(hProcess);
                throw new Win32ApiException($"Failed to obtain a handle for the access token of the winlogon process: {Marshal.GetLastWin32Error()}");
            }

            // Security attibute structure used in DuplicateTokenEx and CreateProcessAsUser
            // I would prefer to not have to use a security attribute variable and to just 
            // simply pass null and inherit (by default) the security attributes
            // of the existing token. However, in C# structures are value types and therefore
            // cannot be assigned the null value.
            SECURITY_ATTRIBUTES sa = new SECURITY_ATTRIBUTES();
            sa.Length = Marshal.SizeOf(sa);

            // copy the access token of the winlogon process; the newly created token will be a primary token
            if (!WindowsInteropApi.DuplicateTokenEx(
                hPToken,
                WindowsInteropConstants.MaximumAllowed,
                ref sa,
                (int)SECURITY_IMPERSONATION_LEVEL.SecurityIdentification,
                (int)TOKEN_TYPE.TokenPrimary,
                ref hUserTokenDup))
            {
                WindowsInteropApi.CloseHandle(hProcess);
                WindowsInteropApi.CloseHandle(hPToken);
                throw new Win32ApiException($"Failed to duplicate the token with error code: {Marshal.GetLastWin32Error()}");
            }

            // By default CreateProcessAsUser creates a process on a non-interactive window station, meaning
            // the window station has a desktop that is invisible and the process is incapable of receiving
            // user input. To remedy this we set the lpDesktop parameter to indicate we want to enable user 
            // interaction with the new process.
            STARTUPINFO si = new STARTUPINFO();
            si.cb = (int)Marshal.SizeOf(si);
            si.lpDesktop = @"winsta0\default"; // interactive window station parameter; basically this indicates that the process created can display a GUI on the desktop

            // flags that specify the priority and creation method of the process
            int dwCreationFlags = WindowsInteropConstants.NormalPriorityClass | WindowsInteropConstants.CreateNewConsole;

            // create a new process in the current user's logon session
            bool result = WindowsInteropApi.CreateProcessAsUser(
                hUserTokenDup,          // client's access token
                null,                   // file to execute
                applicationName,        // command line
                ref sa,                 // pointer to process SECURITY_ATTRIBUTES
                ref sa,                 // pointer to thread SECURITY_ATTRIBUTES
                false,                  // handles are not inheritable
                dwCreationFlags,        // creation flags
                IntPtr.Zero,            // pointer to new environment block 
                null,                   // name of current directory 
                ref si,                 // pointer to STARTUPINFO structure
                out procInfo);          // receives information about new process


            // invalidate the handles
            WindowsInteropApi.CloseHandle(hProcess);
            WindowsInteropApi.CloseHandle(hPToken);
            WindowsInteropApi.CloseHandle(hUserTokenDup);

            if (!result)
            {
                throw new Win32ApiException($"Failed to start process with error code: {Marshal.GetLastWin32Error()}");
            }

            return result; // return the result
        }

        /// <summary>
        /// Returns a string formatted as "domain/username" of the current logged in user.
        /// </summary>
        /// <returns>Logged in user formatted as "domain/username"</returns>
        /// <exception cref="Win32ApiException">If there was a problem geting the current user session token</exception>
        public string GetCurrentLoggedInUsername()
        {
            var hUserToken = IntPtr.Zero;

            try
            {
                if (GetSessionUserToken(ref hUserToken))
                {
                    using (var id = new WindowsIdentity(hUserToken))
                    {
                        return id.Name;
                    }
                }
                else
                {
                    throw new Win32ApiException("Failed getting the current user's token");
                }
            }
            finally
            {
                WindowsInteropApi.CloseHandle(hUserToken);
            }
        }

        /// <summary>
        /// Gets the user token from the currently active session
        /// </summary>
        /// <param name="phUserToken">The reference to the usertoken that will be used</param>
        /// <returns>True if the operation was sucessful</returns>
        internal bool GetSessionUserToken(ref IntPtr phUserToken)
        {
            var bResult = false;
            var hImpersonationToken = IntPtr.Zero;
            var activeSessionId = WindowsInteropConstants.InvalidateSessionId;
            var pSessionInfo = IntPtr.Zero;
            var wtsCurrentServerHandle = IntPtr.Zero;
            var sessionCount = 0;

            // Get a handle to the user access token for the current active session.
            if (WindowsInteropApi.WTSEnumerateSessions(wtsCurrentServerHandle, 0, 1, ref pSessionInfo, ref sessionCount) != 0)
            {
                var arrayElementSize = Marshal.SizeOf(typeof(WTS_SESSION_INFO));
                var current = pSessionInfo;

                for (var i = 0; i < sessionCount; i++)
                {
                    var si = (WTS_SESSION_INFO)Marshal.PtrToStructure((IntPtr)current, typeof(WTS_SESSION_INFO));
                    current += arrayElementSize;

                    if (si.State == WTS_CONNECTSTATE.WTSActive)
                    {
                        activeSessionId = si.SessionID;
                    }
                }
            }

            // If enumerating did not work, fall back to the old method
            if (activeSessionId == WindowsInteropConstants.InvalidateSessionId)
            {
                activeSessionId = WindowsInteropApi.WTSGetActiveConsoleSessionId();
            }

            if (WindowsInteropApi.WTSQueryUserToken(activeSessionId, ref hImpersonationToken) != 0)
            {
                //TODO: Look into just using null
                SECURITY_ATTRIBUTES sa = new SECURITY_ATTRIBUTES();
                sa.Length = Marshal.SizeOf(sa);

                // Convert the impersonation token to a primary token
                bResult = WindowsInteropApi.DuplicateTokenEx(
                    hImpersonationToken,
                    0,
                    ref sa,
                    (int)SECURITY_IMPERSONATION_LEVEL.SecurityImpersonation,
                    (int)TOKEN_TYPE.TokenPrimary,
                    ref phUserToken);

                WindowsInteropApi.CloseHandle(hImpersonationToken);
            }
            else
            {
                throw new Win32ApiException($"WTSQueryUserToken failed: {Marshal.GetLastWin32Error()}");
            }

            return bResult;
        }

        /// <summary>
        /// Brings the window to the front of any other window.
        /// </summary>
        /// <param name="windowHdl">Pointer for the window that should be brought to the front</param>
        public void SendWindowToFront(IntPtr windowHdl)
        {
            WindowsInteropApi.SetWindowPos(
                windowHdl,
                topMostPtr,
                0,
                0,
                0,
                0,
                WindowsInteropConstants.SWPNOSIZE | WindowsInteropConstants.SWPNOMOVE);
        }

        /// <summary>
        /// Handle for the windows task bar. Windows 7 has two handles, one for the taskbar and another for the start button
        /// </summary>
        private List<int> TaskbarHandles
        {
            get
            {
                List<int> taskbarHandles = new List<int>();
                int taskbarHandle = WindowsInteropApi.FindWindow("Shell_TrayWnd", string.Empty);
                int startbuttonHandle = WindowsInteropApi.FindWindowEx(taskbarHandle, IntPtr.Zero, "Button", "Start");

                if (taskbarHandle > 0)
                {
                    taskbarHandles.Add(taskbarHandle);
                }

                if (startbuttonHandle > 0)
                {
                    taskbarHandles.Add(startbuttonHandle);
                }

                return taskbarHandles;
            }
        }

        /// <summary>
        /// Show the Windows taskbar
        /// </summary>
        public void ShowTaskbar()
        {
            TaskbarHandles.ForEach(handle => WindowsInteropApi.ShowWindow(handle, WindowsInteropConstants.ShowWindowShow));
        }

        /// <summary>
        /// Hide the Windows taskbar
        /// </summary>
        public void HideTaskbar()
        {
            TaskbarHandles.ForEach(handle => WindowsInteropApi.ShowWindow(handle, WindowsInteropConstants.ShowWindowHide));
        }

        /// <summary>
        /// Sends a signal to windows to inform it that this application should keep the system awake and unlocked.
        /// This is generally used for media applications where a user does not want the screen to turn off.
        /// </summary>
        public void PreventSleepAndAutolock()
        {
            WindowsInteropApi.SetThreadExecutionState(
                ExecutionState.ES_DISPLAY_REQUIRED
                | ExecutionState.ES_CONTINUOUS
                | ExecutionState.ES_AWAYMODE_REQUIRED
                | ExecutionState.ES_SYSTEM_REQUIRED);
        }

        /// <summary>
        /// Resets the current application execution state. This will allow the system to automatically 
        /// sleep and dim its display as usual.
        /// </summary>
        public void ResetApplicationExecutionState()
        {
            WindowsInteropApi.SetThreadExecutionState(ExecutionState.ES_CONTINUOUS);
        }
    }
}
