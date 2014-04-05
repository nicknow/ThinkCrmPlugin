using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkCrmPlugin.Core.PluginAttributes
{
    [System.AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    class MaxDepthAttribute : Attribute
    {
        private readonly int _maxDepth;

        public MaxDepthAttribute(int maxDepth = 1)
        {
            _maxDepth = maxDepth;
        }         

        public int MaxDepth
        {
            get { return _maxDepth; }
        }
    }
}
