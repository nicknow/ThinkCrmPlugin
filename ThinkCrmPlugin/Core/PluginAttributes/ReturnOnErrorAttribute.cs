using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkCrmPlugin.Core.PluginAttributes
{
    [System.AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    internal class ReturnOnErrorAttribute : Attribute
    {
        private readonly bool _returnOnPluginError;
        private readonly bool _returnOnUnhandledError;
        private readonly bool _returnOnPluginValidationError;

        public ReturnOnErrorAttribute(bool returnOnPluginError = false, bool returnOnUnhandledError = false, bool returnOnPluginValidationError = false)
        {
            _returnOnPluginError = returnOnPluginError;
            _returnOnUnhandledError = returnOnUnhandledError;
            _returnOnPluginValidationError = returnOnPluginValidationError;
        }         

        public bool ReturnOnPluginError
        {
            get { return _returnOnPluginError; }
        }

        public bool ReturnOnUnhandledError
        {
            get { return _returnOnUnhandledError; }
        }

        public bool ReturnOnPluginValidationError
        {
            get { return _returnOnPluginValidationError; }
        }
    }

}
