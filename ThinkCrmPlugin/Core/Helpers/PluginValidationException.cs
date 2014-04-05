using System;

namespace ThinkCrmPlugin.Core.Helpers
{
    public class PluginValidationException : Exception
    {
        public PluginValidationException()
        {
        }

        public PluginValidationException(string message)
            : base(message)
        {
        }

        public PluginValidationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}