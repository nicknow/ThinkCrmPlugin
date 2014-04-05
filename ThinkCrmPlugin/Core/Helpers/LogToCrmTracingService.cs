using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using ThinkCrmPlugin.Core.Interfaces;

namespace ThinkCrmPlugin.Core.Helpers
{
    public class LogToCrmTracingService : ILoggingListener
    {
        private readonly ITracingService _tracingService;

        public LogToCrmTracingService(ITracingService tracingService)
        {
            if (tracingService == null) throw new ArgumentNullException("tracingService");
            _tracingService = tracingService;
        }

        public void Write(string message, string messageUnformatted, params object[] args)
        {
            if (_tracingService != null) _tracingService.Trace(message);
        }
    }
}
