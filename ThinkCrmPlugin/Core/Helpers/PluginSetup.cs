using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using ThinkCrmPlugin.Core.Interfaces;

namespace ThinkCrmPlugin.Core.Helpers
{
    public class PluginSetup : IPluginSetup
    {
        private IPluginExecutionContext _context;
        private readonly IServiceProvider _serviceProvider;
        private IOrganizationServiceFactory _serviceFactory;
        private IOrganizationService _service;
        private ITracingService _tracing;

        public PluginSetup(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null) throw new ArgumentNullException("serviceProvider");
            this._serviceProvider = serviceProvider;
        }

        public IServiceProvider ServiceProvider
        {
            get { return _serviceProvider; }
        }

        public IPluginExecutionContext Context
        {
            get
            {
                if (_context != null)
                {
                    return _context;
                }
                else
                {
                    _context =
                        (Microsoft.Xrm.Sdk.IPluginExecutionContext)
                            ServiceProvider.GetService(typeof(Microsoft.Xrm.Sdk.IPluginExecutionContext));
                    return _context;
                }
            }
        }

        public IOrganizationServiceFactory ServiceFactory
        {
            get
            {
                if (_serviceFactory != null)
                {
                    return _serviceFactory;
                }
                else
                {
                    _serviceFactory =
                        (IOrganizationServiceFactory)
                            ServiceProvider.GetService(typeof(IOrganizationServiceFactory));
                    return _serviceFactory;
                }
            }
        }

        public IOrganizationService ServiceCallingUser
        {
            get
            {
                if (_service != null)
                {
                    return _service;
                }
                else
                {
                    _service = GetNewService(Context.UserId);
                    return _service;
                }
            }
        }

        public IOrganizationService GetNewService(Guid? userId)
        {
            return ServiceFactory.CreateOrganizationService(Context.UserId);
        }

        public ITracingService Tracing
        {
            get
            {
                if (_tracing != null)
                {
                    return _tracing;
                }
                else
                {
                    _tracing = (ITracingService)ServiceProvider.GetService(typeof(ITracingService));
                    return _tracing;
                }
            }
        }
    }    
}
