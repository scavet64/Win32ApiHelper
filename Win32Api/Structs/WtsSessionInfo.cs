using Win32ApiCore.WindowsInteropEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Win32ApiCore.WindowsInteropStructs
{
    /// <summary>
    /// Contains information about a client session
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct WTS_SESSION_INFO
    {
        /// <summary>
        /// Session identifier of the session.
        /// </summary>
        public readonly uint SessionID;

        /// <summary>
        /// Pointer to a null-terminated string that contains the WinStation name of this session. 
        /// The WinStation name is a name that Windows associates with the session, for example, 
        /// "services", "console", or "RDP-Tcp#0".
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public readonly string PWinStationName;

        /// <summary>
        /// A value from the WTS_CONNECTSTATE_CLASS enumeration type that indicates the session's current connection state.
        /// </summary>
        public readonly WTS_CONNECTSTATE State;
    }
}
