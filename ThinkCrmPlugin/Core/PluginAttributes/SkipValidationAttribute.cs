using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkCrmPlugin.Core.PluginAttributes
{
    [System.AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    class SkipValidationAttribute : Attribute
    {
        private readonly bool _skipValidation;

        public SkipValidationAttribute(bool skipValidation = true)
        {
            _skipValidation = skipValidation;
        }

        public bool SkipValidation { get { return _skipValidation; } }
    }
}
