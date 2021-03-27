using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Win32ApiCore.Win32Api
{
    /// <summary>
    /// Exception for the Win32Api
    /// </summary>
    [Serializable]
    public class Win32ApiException : Exception
    {
        /// <summary>
        /// Constructor for Win32ApiException
        /// </summary>
        /// <param name="message">The message to display</param>
        public Win32ApiException(string message) : base(message) { }
    }
}
