using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using ThinkCrmPlugin.Core.Helpers;

namespace ThinkCrmPlugin.Core.PluginAttributes
{

    [System.AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    class PluginRegistrationAttribute : Attribute
    {
        private readonly MessageType _messageType;
        private readonly string _primaryEntityLogicalName;
        private readonly string _secondaryEntityLogicalName;
        private readonly string[] _filteringAttributes;
        private readonly string _name;
        private readonly string _userContext;
        private readonly int _executionOrder;
        private readonly PipelineStage _pipelineStage;
        private readonly ExecutionMode _executionMode;
        private readonly SupportedDeployment _supportedDeployment;
        private readonly bool _deleteAsyncOpOnSuccess;
        private readonly string _preImageName;
        private readonly string[] _preImageAttributes;
        private readonly string _postImageName;
        private readonly string[] _postImageAttributes;
        private readonly string _keyName;

        public PluginRegistrationAttribute(string keyName, MessageType messageType, string primaryEntityLogicalName = null,
            string secondaryEntityLogicalName = null, string[] filteringAttributes = null, string name = null,
            string userContext = null, int executionOrder = 1,
            PipelineStage pipelineStage = PipelineStage.Preoperation,
            ExecutionMode executionMode = ExecutionMode.Synchronous,
            SupportedDeployment supportedDeployment = SupportedDeployment.ServerOnly,
            bool deleteAsyncOpOnSuccess = false, string preImageName = null, string[] preImageAttributes = null,
            string postImageName = null, string[] postImageAttributes = null)
        {            
            _messageType = messageType;
            _primaryEntityLogicalName = primaryEntityLogicalName;
            _secondaryEntityLogicalName = secondaryEntityLogicalName;
            _filteringAttributes = filteringAttributes;
            _name = name;
            _userContext = userContext;
            _executionOrder = executionOrder;
            _pipelineStage = pipelineStage;
            _executionMode = executionMode;
            _supportedDeployment = supportedDeployment;
            _deleteAsyncOpOnSuccess = deleteAsyncOpOnSuccess;
            _preImageName = preImageName;
            _preImageAttributes = preImageAttributes;
            _postImageName = postImageName;
            _postImageAttributes = postImageAttributes;
            _keyName = keyName;
        }

        public bool MatchesExecutingPlugin(MessageType messageType, string primaryEntityName, string secondaryEntityName,
            PipelineStage pipelineStage, ExecutionMode executionMode
            , bool isExecutingOffline, string preImageName, Entity preImage, string postImageName,
            Entity postImage)
        {
            if (_messageType != messageType) return false;

            if (!string.IsNullOrEmpty(_primaryEntityLogicalName) &&
                string.Equals(_primaryEntityLogicalName, primaryEntityName, StringComparison.InvariantCultureIgnoreCase))
                return false;
            if (secondaryEntityName != _secondaryEntityLogicalName) return false;

            if (_pipelineStage != pipelineStage) return false;
            if (_executionMode != executionMode) return false;
            if (_supportedDeployment == SupportedDeployment.MicrosoftDynamicsCrmClientForOutlookOnly &&
                !isExecutingOffline) return false;
            if (_supportedDeployment == SupportedDeployment.ServerOnly && isExecutingOffline) return false;

            if (!string.IsNullOrEmpty(_preImageName) && _preImageName != preImageName) return false;
            if (!string.IsNullOrEmpty(_postImageName) && _postImageName != preImageName) return false;

            if (_preImageAttributes != null && !ImageIncludes(_preImageAttributes, preImage)) return false;

            if (_postImageAttributes != null && !ImageIncludes(_postImageAttributes, postImage)) return false;

            return true;

        }

        private static bool ImageIncludes(IEnumerable<string> imageAttributes, Entity preImage)
        {
            return imageAttributes.All(preImage.Contains);
        }

        public string KeyName { get { return _keyName; } }

    }
}
