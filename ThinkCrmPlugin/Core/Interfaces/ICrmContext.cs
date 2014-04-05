using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using ThinkCrmPlugin.Core.Helpers;
using ThinkCrmPlugin.Modifiable;

namespace ThinkCrmPlugin.Core.Interfaces
{
    public interface ICrmContext
    {

        IPluginSetup PluginSetup { get; }

        IPluginExecutionContext PluginExecutionContext { get; }

        MessageType Message { get; }
        PipelineStage PipelineStage { get; }
        ExecutionMode ExecutionMode { get; }
        IsolationMode IsolationMode { get; }

        ICrmService GetCrmService(Guid? id, bool throwErrors = true);
        void SetCrmService(ICrmService crmService);
        ICrmService CrmService { get; }
        ICrmService ElevatedCrmService { get; }

        ILogging Logging { get; }

        ICrmResourceStrings CrmResourceStrings { get; }

        ICrmHelper CrmHelper { get; }
        ICrmHelper ElevatedCrmHelper { get; }

    }


}
