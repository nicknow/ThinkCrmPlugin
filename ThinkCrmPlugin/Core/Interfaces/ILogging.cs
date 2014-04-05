using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace ThinkCrmPlugin.Core.Interfaces
{
    public interface ILogging
    {

        void Write(string message, params object[] args);

        T WriteAndReturn<T>(T returnItem, string message, params object[] args);

        void Write(Exception ex);

        void If(bool condition, string message, params object[] args);

        void IfNot(bool condition, string message, params object[] args);
        
        void Debug(string message, params object[] args);

        void DebugIf(bool condition, string message, params object[] args);

        void DebugIfNot(bool condition, string message, params object[] args);
       
        void AddListener(ILoggingListener listener);

        void ClearListeners();

    }

}
