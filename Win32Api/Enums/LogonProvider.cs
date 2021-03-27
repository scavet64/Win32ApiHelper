using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Win32ApiCore.WindowsInteropEnums
{
    /// <summary>
    /// Specifies the logon provider. This parameter can be one of the following values.
    /// </summary>
    public enum LogonProvider
    {
        /// <summary>
        /// Use the standard logon provider for the system. 
        /// The default security provider is negotiate, unless you pass NULL for the domain name and 
        /// the user name is not in UPN format. In this case, the default provider is NTLM.
        /// </summary>
        LOGON32_PROVIDER_DEFAULT = 0,
        LOGON32_PROVIDER_WINNT35 = 1,
        /// <summary>
        /// Use the NTLM logon provider.
        /// </summary>
        LOGON32_PROVIDER_WINNT40 = 2,
        /// <summary>
        /// Use the negotiate logon provider.
        /// </summary>
        LOGON32_PROVIDER_WINNT50 = 3
    }
}
