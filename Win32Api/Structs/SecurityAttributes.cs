using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Win32ApiCore.WindowsInteropStructs
{
    /// <summary>
    /// The SECURITY_ATTRIBUTES structure contains the security descriptor for an object 
    /// and specifies whether the handle retrieved by specifying this structure is inheritable. 
    /// <para>
    ///     For more information see https://docs.microsoft.com/en-us/previous-versions/windows/desktop/legacy/aa379560(v=vs.85)
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SECURITY_ATTRIBUTES
    {
        /// <summary>
        /// The size, in bytes, of this structure. Set this value to the size of the SECURITY_ATTRIBUTES structure
        /// </summary>
        public int Length;
        /// <summary>
        /// A pointer to a SECURITY_DESCRIPTOR structure that controls access to the object. If the value of this 
        /// member is NULL, the object is assigned the default security descriptor associated with the access token
        /// of the calling process. This is not the same as granting access to everyone by assigning a NULL
        /// discretionary access control list (DACL). By default, the default DACL in the access token of a process 
        /// allows access only to the user represented by the access token.
        /// </summary>
        public IntPtr lpSecurityDescriptor;
        /// <summary>
        /// A Boolean value that specifies whether the returned handle is inherited when a new process is created. 
        /// If this member is TRUE, the new process inherits the handle.
        /// </summary>
        public bool bInheritHandle;
    }
}
