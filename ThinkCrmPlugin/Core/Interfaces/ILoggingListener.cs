using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkCrmPlugin.Core.Interfaces
{
    public interface ILoggingListener
    {
        /// <summary>
        /// Called when a message should be written to the log. 
        /// </summary>        
        /// <param name="message">A fully formatted message that can be written directly to the log with no further processing.</param>
        /// <param name="logType">The message level to be logged. The method will only be called when the the message is at or above the message level the listener was registered at.</param>        
        /// <param name="messageUnformatted">This is a string to be used with the passed <paramref name="args"/> in the manned used by string.format().</param>
        /// <param name="args"></param>
        void Write(string message, string messageUnformatted, params object[] args);
    }
}
