using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using ThinkCrmPlugin.Core.Helpers;
using ThinkCrmPlugin.Core.Interfaces;
using ThinkCrmPlugin.Modifiable;

namespace ThinkCrmPlugin.Core.CorePlugins
{
    public class CrmContext : ICrmContext
    {
        private readonly IPluginSetup _pluginSetup;
        private readonly IPluginExecutionContext _pluginExecutionContext;
        private readonly MessageType _message;
        private readonly PipelineStage _pipelineStage;
        private readonly ExecutionMode _executionMode;
        private readonly IsolationMode _isolationMode;                
        private readonly ILogging _logging;
        private readonly ICrmResourceStrings _crmResourceStrings;

        private ICrmService _elevatedCrmService;
        private ICrmService _crmService;
        private ICrmHelper _crmHelper;
        private ICrmHelper _elevatedCrmHelper;        

        public static ICrmContext GenerateContext(IPluginSetup pluginSetup, ILogging logging, ICrmResourceStrings resourceString)
        {
            return new CrmContext(pluginSetup, logging, resourceString);
        }

        private CrmContext(IPluginSetup pluginSetup, ILogging logging, ICrmResourceStrings crmResourceStrings)
        {
            if (pluginSetup == null) throw new ArgumentNullException("pluginSetup");
            if (logging == null) throw new ArgumentNullException("logging");

            _pluginSetup = pluginSetup;
            _logging = logging;
            _crmResourceStrings = crmResourceStrings;

            _pluginExecutionContext = _pluginSetup.Context;

            try
            {
                _message = ParseEnum<MessageType>(_pluginExecutionContext.MessageName);
                _pipelineStage = (PipelineStage)_pluginExecutionContext.Stage;
                _executionMode = (ExecutionMode) _pluginExecutionContext.Mode;
                _isolationMode = (IsolationMode) _pluginExecutionContext.IsolationMode;

            }
            catch (Exception)
            {
                _logging.Write(
                    GetString("LoggingCrmSetupParsingException",
                        "ThinkCrmPlugin: Exception when Parsing Plugin Execution Values: Message={0} / Stage={1} / ExecutionMode={2} / IsolationMode={3}"),
                    _pluginExecutionContext.MessageName, _pluginExecutionContext.Stage, _pluginExecutionContext.Mode,
                    _pluginExecutionContext.IsolationMode);
                throw;
            }            
            

        }

        #region Simple Properties

        public IPluginSetup PluginSetup
        {
            get { return _pluginSetup; }
        }

        public IPluginExecutionContext PluginExecutionContext
        {
            get { return _pluginExecutionContext; }
        }

        public MessageType Message
        {
            get { return _message; }
        }

        public PipelineStage PipelineStage
        {
            get { return _pipelineStage; }
        }

        public ExecutionMode ExecutionMode
        {
            get { return _executionMode; }
        }

        public IsolationMode IsolationMode
        {
            get { return _isolationMode; }
        }

        public ILogging Logging
        {
            get { return _logging; }
        }

        public ICrmResourceStrings CrmResourceStrings
        {
            get { return _crmResourceStrings; }
        }           

        #endregion

        #region Complex Properties and Methods
        public ICrmService GetCrmService(Guid? id, bool throwErrors)
        {
            _logging.Write(GetString("LoggingCrmServiceCreateService", "GetCrmService Id: {0}"), id.ToString());
            return new CrmService(_pluginSetup.GetNewService(id), _logging, throwErrors, _crmResourceStrings);
        }

        public void SetCrmService(ICrmService crmService)
        {
            if (crmService == null) throw new ArgumentNullException("crmService");
            _crmService = crmService;
            _logging.Write(GetString("LoggingCrmContextLazyCreate", "ThinkCrmPlugin: Creating {0}"), "SetCrmService");
            if (_crmHelper != null) _crmHelper = new CrmHelper(_crmService,_logging,_crmResourceStrings);
        }

        public ICrmService CrmService
        {
            get
            {
                if (_crmService != null) return _crmService;
                _logging.Write(GetString("LoggingCrmContextLazyCreate", "ThinkCrmPlugin: Creating {0}"), "CrmService");
                _crmService = this.GetCrmService(_pluginExecutionContext.InitiatingUserId, true);
                return _crmService;
            }
        }

        public ICrmService ElevatedCrmService
        {
            get
            {
                if (_elevatedCrmService != null) return _elevatedCrmService;
                _logging.Write(GetString("LoggingCrmContextLazyCreate", "ThinkCrmPlugin: Creating {0}"), "ElevatedCrmService");
                _elevatedCrmService = this.GetCrmService(null, true);
                return _elevatedCrmService;
            }
        }

        public ICrmHelper CrmHelper
        {
            get
            {
                if (_crmHelper != null) return _crmHelper;
                _logging.Write(GetString("LoggingCrmContextLazyCreate","ThinkCrmPlugin: Creating {0}"), "CrmHelper");
                _crmHelper = new CrmHelper(this.CrmService, _logging, _crmResourceStrings);
                return _crmHelper;
            }
        }

        public ICrmHelper ElevatedCrmHelper
        {
            get
            {
                if (_elevatedCrmHelper != null) return _elevatedCrmHelper;
                _logging.Write(GetString("LoggingCrmContextLazyCreate", "ThinkCrmPlugin: Creating {0}"), "ElevatedCrmHelper");
                _elevatedCrmHelper = new CrmHelper(this.ElevatedCrmService, _logging, _crmResourceStrings);
                return _elevatedCrmHelper;
            }
        } 
        #endregion

        #region Internal Helpers

        private static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        private string GetString(string stringId, string defaultValue)
        {
            return _crmResourceStrings != null ? _crmResourceStrings.GetString(stringId, defaultValue) : defaultValue;
        } 
        #endregion
    }
}