using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkCrmPlugin.Core.Interfaces;

namespace ThinkCrmPlugin.Core.Logging
{
    public class Logger : ILogging
    {
        private readonly bool _isInSandbox;
        private readonly List<ILoggingListener> _listeners;

        public Logger(bool isInSandbox = true)
        {
            _isInSandbox = isInSandbox;
            this._listeners = new List<ILoggingListener>();
        }

        public void Write(string message, params object[] args)
        {

            string compiledMessage = compiledMessage = !args.Any() ? message : string.Format(message, args);

            if (compiledMessage != null)
                _listeners.ForEach(t => t.Write(compiledMessage, message, args));

        }

        public T WriteAndReturn<T>(T returnItem, string message, params object[] args)
        {
            this.Write(message, args);
            return returnItem;
        }

        public void Write(Exception ex)
        {
            this.Write(_isInSandbox
                ? SandboxedExceptionHandling.GetExtendEdexceptionDetails(ex)
                : NonSandboxedExceptionLogging.GetExtendedExceptionDetails(ex));
        }

        public void If(bool condition, string message, params object[] args)
        {
            if (condition) this.Write(message, args);
        }

        public void IfNot(bool condition, string Message, params object[] args)
        {
            if (!condition) this.Write(Message, args);
        }

        public void Debug(string message, params object[] args)
        {
#if DEBUG
            this.Write(message, args);
#else
            return;            
#endif
        }

        public void DebugIf(bool condition, string message, params object[] args)
        {
#if DEBUG
            this.If(condition, message, args);
#else
            return;            
#endif
        }

        public void DebugIfNot(bool condition, string message, params object[] args)
        {
#if DEBUG
            this.IfNot(condition, message, args);
#else
            return;            
#endif
        }

        public void AddListener(ILoggingListener listener)
        {
            if (!_listeners.Contains(listener)) _listeners.Add(listener);
        }

        public void ClearListeners()
        {
            _listeners.Clear();
        }
    }
}
