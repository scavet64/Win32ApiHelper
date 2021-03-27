﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Win32ApiCore.WindowsInteropStructs
{
    /// <summary>
    /// Contains information about a newly created process and its primary thread.
    /// <para>
    ///     For more information see https://docs.microsoft.com/en-us/windows/win32/api/processthreadsapi/ns-processthreadsapi-process_information
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct PROCESS_INFORMATION
    {
        /// <summary>
        /// A handle to the newly created process. The handle is used to specify the 
        /// process in all functions that perform operations on the process object.
        /// </summary>
        public IntPtr hProcess;
        /// <summary>
        /// A handle to the primary thread of the newly created process. The handle 
        /// is used to specify the thread in all functions that perform operations on 
        /// the thread object.
        /// </summary>
        public IntPtr hThread;
        /// <summary>
        /// A value that can be used to identify a process. The value is valid from the 
        /// time the process is created until all handles to the process are closed and 
        /// the process object is freed; at this point, the identifier may be reused.
        /// </summary>
        public uint dwProcessId;
        /// <summary>
        /// A value that can be used to identify a thread. The value is valid from the 
        /// time the thread is created until all handles to the thread are closed and 
        /// the thread object is freed; at this point, the identifier may be reused.
        /// </summary>
        public uint dwThreadId;
    }
}
