using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using ThinkCrmPlugin.Core.CorePlugins;
using ThinkCrmPlugin.Core.Helpers;
using ThinkCrmPlugin.Core.Interfaces;
using ThinkCrmPlugin.Core.PluginAttributes;

namespace ThinkCrmPlugin
{
    [PluginRegistration("MyCreate", MessageType.Create, "Contact")]
    [PluginRegistration("MyUpdate", MessageType.Update, "Contact")]
    public class BasicCrmPlugin : CrmPlugin
    {
        public override void Execute(ICrmContext crmContext, string keyName)
        {
            var target = crmContext.PluginExecutionContext.InputParameters["Target"] as Entity;

            if (target != null && target.Contains("firstname")) target["firstname"] = "Nicolas";
            else if (target != null) target.Attributes.Add(new KeyValuePair<string, object>("firstname","Nicolas"));
        }
        
    }
}
