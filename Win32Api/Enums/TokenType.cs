using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Win32ApiCore.WindowsInteropEnums
{
    /// <summary>
    /// The TOKEN_TYPE enumeration contains values that differentiate between a primary token and an impersonation token.
    /// </summary>
    public enum TOKEN_TYPE : int
    {
        /// <summary>
        /// Indicates a primary token. This is required for this CreateProcessAsUser function
        /// </summary>
        TokenPrimary = 1,
        /// <summary>
        /// Indicates an impersonation token.
        /// </summary>
        TokenImpersonation = 2
    }
}
