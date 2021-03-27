using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Win32ApiCore
{
    /// <summary>
    /// Constants used for the Win32Api interop calls
    /// </summary>
    public class WindowsInteropConstants
    {
        public const uint MaximumAllowed = 0x2000000;
        public const int CreateNewConsole = 0x00000010;

        public const int IdlePriorityClass = 0x40;
        public const int NormalPriorityClass = 0x20;
        public const int HighPriorityClass = 0x80;
        public const int RealtimePriorityClass = 0x100;

        #region Token Duplication/Creation
        public const int SwShow = 5;
        public const int TokenQuery = 0x0008;
        public const int TokenDuplicate = 0x0002;
        public const int TokenAssignPrimary = 0x0001;
        public const int StartFUseShowWindow = 0x00000001;
        public const int StartFForceOnFeedback = 0x00000040;
        public const int CreateUnicodeEnvironment = 0x00000400;
        public const int TokenImpersonate = 0x0004;
        public const int TokenQuerySource = 0x0010;
        public const int TokenAdjustPrivileges = 0x0020;
        public const int TokenAdjustGroups = 0x0040;
        public const int TokenAdjustDefault = 0x0080;
        public const int TokenAdjustSessionId = 0x0100;
        public const int StandardRightsRequired = 0x000F0000;
        public const int TokenAllAccess =
            StandardRightsRequired |
            TokenAssignPrimary |
            TokenDuplicate |
            TokenImpersonate |
            TokenQuery |
            TokenQuerySource |
            TokenAdjustPrivileges |
            TokenAdjustGroups |
            TokenAdjustDefault |
            TokenAdjustSessionId;
        #endregion

        public const int CreateNoWindow = 0x08000000;

        public const uint InvalidateSessionId = 0xFFFFFFFF;

        /// <summary>
        /// Flag to ignore size changes in the uFlags param for setwindowpos
        /// </summary>
        public const uint SWPNOSIZE = 0x1;

        /// <summary>
        /// Flag to ignore position changes in the uFlags param for setwindowpos
        /// </summary>
        public const uint SWPNOMOVE = 0x2;

        public const int ShowWindowHide = 0;
        public const int ShowWindowShow = 1;
    }
}
