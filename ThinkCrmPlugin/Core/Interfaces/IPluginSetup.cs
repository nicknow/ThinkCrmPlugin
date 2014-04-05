using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace ThinkCrmPlugin.Core.Interfaces
{
    public interface IPluginSetup
    {
        IServiceProvider ServiceProvider { get; }
        IPluginExecutionContext Context { get; }
        IOrganizationServiceFactory ServiceFactory { get; }
        IOrganizationService ServiceCallingUser { get; }
        ITracingService Tracing { get; }
        IOrganizationService GetNewService(Guid? userId);
    }
}
