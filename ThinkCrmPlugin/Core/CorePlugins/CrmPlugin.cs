using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.Xrm.Sdk;
using ThinkCrmPlugin.Core.Helpers;
using ThinkCrmPlugin.Core.Interfaces;
using ThinkCrmPlugin.Core.Logging;
using ThinkCrmPlugin.Core.PluginAttributes;

namespace ThinkCrmPlugin.Core.CorePlugins
{
    public abstract class CrmPlugin : IPlugin
    {

        private const string DefaultExceptionString =
            "An error has occurred during this operation. Please click Download Log and provide this to your Help Desk, Dynamics CRM Consultant Support Team, or Dynamics CRM Consultant.";

        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginSetup pluginSetup = new PluginSetup(serviceProvider);

            pluginSetup.Tracing.Trace("ThinkCrmPlugin: Begining Execution of Plugin (CRM Tracing Only): {0}", MethodBase.GetCurrentMethod().DeclaringType);

            var logging = this.InitializeLogging(pluginSetup,
                pluginSetup.Context.IsolationMode == (int)IsolationMode.Sandbox ? true : false);

            logging.Write("ThinkCrmPlugin: Logging Configured", MethodBase.GetCurrentMethod().DeclaringType);

            var errorRules = GetErrorRules(logging);

            #region ResourceString

            ICrmResourceStrings resourceString;

            try
            {
                resourceString = this.InitializeResourceStrings(pluginSetup, logging);
            }
            catch (InvalidPluginExecutionException ex)
            {
                logging.Write("ThinkCrmPlugin: InvalidPluginExectionException During InitializeResourceStrings Operation");
                logging.Write(ex);
                CrmDetailsLogger.LogContext(pluginSetup.Context, logging);
                if (!errorRules.ReturnOnPluginError) throw;
                logging.Write("ThinkCrmPlugin: Error Handling Set to Not Throw on InvalidPluginExecutionException.");
                return;
            }
            catch (Exception ex)
            {
                logging.Write("ThinkCrmPlugin: Unhandled Exeception During InitializeResourceStrings Operation");
                logging.Write(ex);
                CrmDetailsLogger.LogContext(pluginSetup.Context, logging);
                if (!errorRules.ReturnOnUnhandledError) throw new InvalidPluginExecutionException(DefaultExceptionString, ex);
                logging.Write("ThinkCrmPlugin: Error Handling Set to Not Throw on Unhandled Exception.");
                return;
            }
            #endregion

            var crmContext = CrmContext.GenerateContext(pluginSetup, logging, resourceString);

            #region SkipValidation

            bool skipValidation = false;

            try
            {
                skipValidation = SkipPluginValidation(crmContext, logging);
            }            
            catch (Exception ex)
            {
                logging.Write("ThinkCrmPlugin: Unhandled Exeception During SkipValidation Read Operation");
                logging.Write(ex);
                CrmDetailsLogger.LogContext(crmContext.PluginExecutionContext, logging);
                if (!errorRules.ReturnOnUnhandledError)
                    throw new InvalidPluginExecutionException(resourceString != null
                        ? resourceString.GetString("UserMessageUnhandledException", DefaultExceptionString)
                        : DefaultExceptionString, ex);
                logging.Write("ThinkCrmPlugin: Error Handling Set to Not Throw on Unhandled Exception.");
                return;
            }

            #endregion

            #region MaxDepth

            try
            {
                bool exceedsMaxDepth = ExceedsMaxDepth(crmContext.PluginExecutionContext.Depth, logging);
                if (exceedsMaxDepth)
                {
                    logging.Write("ThinkCrmPlugin: Exceeds Max Depth");
                    return;
                }
                else
                {
                    logging.Write("ThinkCrmPlugin: Passed MaxDepth Check");
                }
            }
            catch (Exception ex)
            {
                logging.Write("ThinkCrmPlugin: Unhandled Exeception During MaxDepth Read/Check Operation");
                logging.Write(ex);
                CrmDetailsLogger.LogContext(crmContext.PluginExecutionContext, logging);
                if (!errorRules.ReturnOnUnhandledError) throw new InvalidPluginExecutionException(resourceString != null
                     ? resourceString.GetString("UserMessageUnhandledException", DefaultExceptionString)
                     : DefaultExceptionString, ex);
                logging.Write("ThinkCrmPlugin: Error Handling Set to Not Throw on Unhandled Exception.");
                return;
            }
            #endregion

            #region Plugin Execution Validation
            
            string keyName = string.Empty;

            if (!skipValidation)
            {
                try
                {
                    if (!this.ValidatePluginExecution(crmContext, logging, out keyName))
                    {
                        logging.Write("ThinkCrmPlugin: Failed To Validate for Execution");
                        if (errorRules.ReturnOnPluginValidationError) return;
                        throw new PluginValidationException("ThinkCrmPlugin: Plugin Failed To Validate for Exection.");
                    }
                    logging.Write("ThinkCrmPlugin: Passed Execution Validation KeyName={0}",
                        string.IsNullOrEmpty(keyName) ? "(KeyName Not Defined)" : keyName);
                }
                catch (PluginValidationException ex)
                {
                    logging.Write(ex);
                    CrmDetailsLogger.LogContext(crmContext.PluginExecutionContext, logging);
                    if (!errorRules.ReturnOnPluginValidationError)
                    {
                        throw new InvalidPluginExecutionException(resourceString != null
                            ? resourceString.GetString("UserMessageUnhandledException", DefaultExceptionString)
                            : DefaultExceptionString, ex);
                    }
                    logging.Write("ThinkCrmPlugin: Error Handling Set to Not Throw on Plugin Validation Error");
                    return;
                }
                catch (InvalidPluginExecutionException ex)
                {
                    logging.Write("ThinkCrmPlugin: InvalidPluginExectionException During Plugin Validation Operation");
                    logging.Write(ex);
                    CrmDetailsLogger.LogContext(crmContext.PluginExecutionContext, logging);
                    if (!errorRules.ReturnOnPluginError) throw;
                    logging.Write("ThinkCrmPlugin: Error Handling Set to Not Throw on InvalidPluginExecutionException");
                    return;
                }
                catch (Exception ex)
                {
                    logging.Write("ThinkCrmPlugin: Unhandled Exeception During Plugin Validation Operation");
                    logging.Write(ex);
                    CrmDetailsLogger.LogContext(crmContext.PluginExecutionContext, logging);
                    if (!errorRules.ReturnOnUnhandledError) throw new InvalidPluginExecutionException(resourceString != null
                         ? resourceString.GetString("UserMessageUnhandledException", DefaultExceptionString)
                         : DefaultExceptionString, ex);
                    logging.Write("ThinkCrmPlugin: Error Handling Set to Not Throw on Unhandled Exception");
                    return;
                }
            }
            else
            {
                logging.Write("ThinkCrmPlugin: Skipped Plugin Validation");
            } 
            #endregion

            #region Core Plugin Operation

            try
            {
                logging.Write("ThinkCrmPlugin: Initiating Core Execute Operation");
                CrmDetailsLogger.LogPipeline(crmContext.PluginExecutionContext, logging);
                this.Execute(crmContext, keyName);
                logging.Write("ThinkCrmPlugin: Completed Core Execute Operation");
                return;
            }
            catch (InvalidPluginExecutionException ex)
            {
                logging.Write("ThinkCrmPlugin: InvalidPluginExectionException During Core Execute Operation");
                logging.Write(ex);
                CrmDetailsLogger.LogContext(crmContext.PluginExecutionContext, logging);
                if (!errorRules.ReturnOnPluginError) throw;
                logging.Write("ThinkCrmPlugin: Error Handling Set to Not Throw on InvalidPluginExecutionException.");
                return;                        
            }
            catch (Exception ex)
            {
                logging.Write("ThinkCrmPlugin: Unhandled Exeception During Core Execute Operation");
                logging.Write(ex);
                CrmDetailsLogger.LogContext(crmContext.PluginExecutionContext, logging);
                if(!errorRules.ReturnOnUnhandledError) throw new InvalidPluginExecutionException(resourceString != null
                    ? resourceString.GetString("UserMessageUnhandledException", DefaultExceptionString)
                    : DefaultExceptionString, ex);
                logging.Write("ThinkCrmPlugin: Error Handling Set to Not Throw on Unhandled Exception.");
                return;                        
            }

            #endregion

        }

        protected virtual ICrmResourceStrings InitializeResourceStrings(IPluginSetup pluginSetup, ILogging logging)
        {
            logging.Write("ThinkCrmPlugin: No Resource Strings Configured");
            return null;
        }

        protected virtual ILogging InitializeLogging(IPluginSetup pluginSetup, bool isInSandbox = true)
        {
            var log = new Logger(isInSandbox);
            log.AddListener(new LogToCrmTracingService(pluginSetup.Tracing));
            return log.WriteAndReturn(log, "ThinkCrmPlugin: Default Logging / Sandbox Mode={0}", isInSandbox.ToString());            
        }

        protected virtual void AddLoggers(ILogging logging, IPluginSetup pluginSetup)
        {
            return;            
        }

        protected virtual bool ValidatePluginExecution(ICrmContext crmContext, ILogging logging, out string keyName)
        {
            logging.Write("ThinkCrmPlugin: Default ValidatePluginExecution Executed");            

            var attrs = System.Attribute.GetCustomAttributes(this.GetType());

            var preImageName = crmContext.PluginExecutionContext.PreEntityImages.Count > 0
                ? crmContext.PluginExecutionContext.PreEntityImages.First().Key : string.Empty;
            var preImage = crmContext.PluginExecutionContext.PreEntityImages.Count > 0
                ? crmContext.PluginExecutionContext.PreEntityImages.First().Value
                : null;

            var postImageName = crmContext.PluginExecutionContext.PostEntityImages.Count > 0
                ? crmContext.PluginExecutionContext.PostEntityImages.First().Key
                : string.Empty;
            var postImage = crmContext.PluginExecutionContext.PostEntityImages.Count > 0
                ? crmContext.PluginExecutionContext.PostEntityImages.First().Value
                : null;


            foreach (var pAtt in attrs.OfType<PluginRegistrationAttribute>().Select(att => att as PluginRegistrationAttribute))
            {             
                logging.Write("ThinkCrmPlugin: Validating Key Name: {0}", pAtt.KeyName);
                if (pAtt.MatchesExecutingPlugin(crmContext.Message, crmContext.PluginExecutionContext.PrimaryEntityName,
                    crmContext.PluginExecutionContext.SecondaryEntityName, crmContext.PipelineStage,
                    crmContext.ExecutionMode, crmContext.PluginExecutionContext.IsExecutingOffline, preImageName,
                    preImage, postImageName, postImage))
                {
                    keyName = pAtt.KeyName;
                    logging.WriteAndReturn(true, "ThinkCrmPlugin: Validated Key Name: {0}", keyName);                                       
                }
                else
                {
                    logging.Write("ThinkCrmPlugin: Validation Failed for Key Name: {0}", pAtt.KeyName);
                }

            }

            logging.Write("ThinkCrmPlugin: No PluginRegistrationAttribute was Validated");
            keyName = string.Empty;
            return false;
        }

        private bool SkipPluginValidation(ICrmContext crmContext, ILogging logging)
        {
            logging.Write("ThinkCrmPlugin: SkipPluginValidation Executed");            

            var attrs = System.Attribute.GetCustomAttributes(this.GetType());

            if (!attrs.OfType<SkipValidationAttribute>().Any()) return logging.WriteAndReturn(false, "ThinkCrmPlugin: No SkipValidationAttribute Attribute Found");

            var skipBool = attrs.OfType<SkipValidationAttribute>().First().SkipValidation;

            return logging.WriteAndReturn(skipBool, "ThinkCrmPlugin: SkipPluginValidation Attribute Found, Skip={0}", skipBool.ToString());

        }

        private bool ExceedsMaxDepth(int pluginDepth, ILogging logging)
        {
            logging.Write("ThinkCrmPlugin: ExceedsMaxDepth Executed");

            var attrs = System.Attribute.GetCustomAttributes(this.GetType());

            if (!attrs.OfType<MaxDepthAttribute>().Any()) return logging.WriteAndReturn(false, "ThinkCrmPlugin: No MaxDepth Attribute Found");

            var maxAllowedDepth = attrs.OfType<MaxDepthAttribute>().First().MaxDepth;

            logging.Write("ThinkCrmPlugin: MaxDepth Attribute Found, MaxDepth Allowed={0} Actual Depth={1}",maxAllowedDepth, pluginDepth);

            return pluginDepth > maxAllowedDepth;
        }

        private ReturnOnErrorAttribute GetErrorRules(ILogging logging)
        {
            logging.Write("ThinkCrmPlugin: GetErrorRules Executed");

            var attrs = System.Attribute.GetCustomAttributes(this.GetType());

            if (!attrs.OfType<ReturnOnErrorAttribute>().Any()) return logging.WriteAndReturn(new ReturnOnErrorAttribute(), "ThinkCrmPlugin: No MaxDepth Attribute Found, Using Defaults");

            var errorRules = (ReturnOnErrorAttribute) attrs.First();

            return logging.WriteAndReturn(errorRules,
                "ThinkCrmPlugin: Found ReturnOnError Attribute ReturnOnPluginError={0} / ReturnOnPluginValiationError={1} / ReturnOnUnhandledError={2} ",
                errorRules.ReturnOnPluginError.ToString(), errorRules.ReturnOnPluginValidationError.ToString(),
                errorRules.ReturnOnUnhandledError.ToString());

        }

        public abstract void Execute(ICrmContext crmContext, string keyName);
    }
}
