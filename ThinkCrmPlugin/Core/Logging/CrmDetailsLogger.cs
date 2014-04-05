using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using ThinkCrmPlugin.Core.Helpers;
using ThinkCrmPlugin.Core.Interfaces;

namespace ThinkCrmPlugin.Core.Logging
{
    public static class CrmDetailsLogger
    {
        public const string SEPERATOR = "--------------------";

        public static void LogContext(IPluginExecutionContext Context, ILogging log)
        {
            log.Write("ThinkCrmPlugin Detail Start");

            LogContext(Context, log, 0);

            log.Write("ThinkCrmPlugin Detail End ---");
        }

        public static void LogPipeline(IPluginExecutionContext Context, ILogging log)
        {

            log.Write("Is Running Sandbox: \t {0}", IsInSandbox(Context) ? "Yes" : "No");
            log.Write("Context Type: \t\t {0}", Context.GetType().ToString());
            log.Write("Message Name: \t\t {0}", Context.MessageName);
            log.Write("Primary Entity Id: \t {0}", Context.PrimaryEntityId.ToString());
            log.Write("Primary Entity Name: \t {0}", Context.PrimaryEntityName);
            log.Write("Mode: \t\t\t {0}", GetEnum<ExecutionMode>(Context.Mode));
            log.Write("Stage: \t\t\t {0}", GetEnum<PipelineStage>(Context.Stage));
            log.Write("User Id: \t\t {0}", Context.UserId);
            log.Write("Depth: \t\t\t {0}", Context.Depth);
        }

        private static void LogContext(IPluginExecutionContext Context, ILogging l,
            int contextLevel)
        {
            var log = l;

            log.Write("--- Context Level {0} ---", contextLevel);
            log.Write("--- {0} ---", contextLevel == 0 ? "Primary Context" : "Parent Context");
            log.Write(SEPERATOR);
            log.Write("Context Type: \t\t {0}", Context.GetType().ToString());
            log.Write("Business Unit Id: \t {0}", Context.BusinessUnitId.ToString());
            log.Write("Correlation Id: \t {0}", Context.CorrelationId.ToString());
            log.Write("Depth: \t\t\t {0}", Context.Depth);
            log.Write("Initiating User Id: \t {0}", Context.InitiatingUserId);
            log.Write("Is Executing Offline: \t {0}", BoolToString(Context.IsExecutingOffline));
            log.Write("Is In Transaction: \t {0}", BoolToString(Context.IsInTransaction));
            log.Write("Is Offline Playback: \t {0}", BoolToString(Context.IsOfflinePlayback));
            log.Write("Isolation Mode: \t {0} ({1})", GetEnum<IsolationMode>(Context.IsolationMode),
                Context.IsolationMode);
            log.Write("Message Name: \t\t {0}", Context.MessageName);
            log.Write("Mode: \t\t\t {0} ({1})", GetEnum<ExecutionMode>(Context.Mode), Context.Mode);
            log.Write("Operation Created On: \t {0}", Context.OperationCreatedOn.ToString());
            log.Write("Operation Id: \t\t {0}", Context.OperationId.ToString());
            log.Write("Organization Id: \t {0}", Context.OrganizationId.ToString());
            log.Write("Organization Name: \t {0}", Context.OrganizationName);
            log.Write("Owning Extension: \t {0}", LogEntityReference(Context.OwningExtension));
            log.Write("Primary Entity Id: \t {0}", Context.PrimaryEntityId.ToString());
            log.Write("Priamry Entity Name: \t {0}", Context.PrimaryEntityName);
            log.Write("Request Id: \t\t {0}", Context.RequestId.ToString());
            log.Write("Secondary Entity Name: \t {0}", Context.SecondaryEntityName);
            log.Write("Stage: \t\t\t {0} ({1})", GetEnum<PipelineStage>(Context.Stage), Context.Stage);
            log.Write("User Id: \t\t {0}", Context.UserId);

            LogParameters(Context.InputParameters, l, "Input Parameters");

            LogParameters(Context.OutputParameters, l, "Output Parameters");

            LogImages(Context.PreEntityImages, l, "Pre Entity Images");

            LogImages(Context.PostEntityImages, l, "Post Entity Images");

            LogParameters(Context.SharedVariables, l, "Shared Variables");

            if (Context.ParentContext != null)
            {
                contextLevel += 1;
                log.Write(SEPERATOR);
                LogContext(Context.ParentContext, l, contextLevel);
            }

        }

        private static void LogImages(EntityImageCollection images, ILogging log, string header)
        {

            log.Write("--- {0} ---", header);
            log.Write(SEPERATOR);

            if (images.Count > 0)
                images.ToList().ForEach(img =>
                {
                    log.Write("Key: \t\t {0}", img.Key);
                    LogEntity(img.Value, log);
                });

            log.Write(SEPERATOR);
        }

        public static void LogParameters(ParameterCollection Params, ILogging log, string header)
        {

            log.Write("--- {0} ---", header);
            log.Write(SEPERATOR);

            if (Params.Count > 0)
            {

                foreach (var item in Params)
                {
                    log.Write("Item Key: \t {0}", item.Key);
                    log.Write("Item Type: \t {0}", item.Value.GetType().Name);

                    if (item.Value is Entity)
                    {
                        LogEntity((Entity) item.Value, log);
                    }
                    else if (item.Value is EntityReference)
                    {
                        log.Write("Item Value: {0}", LogEntityReference((EntityReference) item.Value));
                    }
                    else if (item.Value is EntityCollection)
                    {
                        log.Write("--- Entities in Collection ---");
                        ((EntityCollection) item.Value).Entities.ToList().ForEach(ent => LogEntity(ent, log));
                    }
                    else if (item.Value is OrganizationRequest)
                    {
                        var req = (OrganizationRequest) item.Value;
                        LogOrganizationRequest(log, req);
                    }
                    else if (item.Value is OrganizationResponse)
                    {
                        var req = (OrganizationResponse) item.Value;
                        LogOrganizationResponse(log, req);
                    }
                    else
                    {
                        log.Write("Item Value: \t {0}", item.Value.ToString());
                    }
                }
            }
            else
            {
                log.Write(string.Format("No {0}", header));
            }

            log.Write(SEPERATOR);
        }

        public static void LogOrganizationResponse(ILogging log, OrganizationResponse req)
        {
            log.Write("Response Name: \t {0}", req.ResponseName);
            log.Write("Response Id: \t {0}", req.ResponseName);
            LogParameters(req.Results, log, "Response Results");
        }

        public static void LogOrganizationRequest(ILogging log, OrganizationRequest req)
        {
            log.Write("Request Name: \t {0}", req.RequestName);
            log.Write("Request Id: \t {0}", req.RequestId);
            LogParameters(req.Parameters, log, "Request Parameters");
        }

        public static void LogEntity(Entity ent, ILogging log)
        {

            log.Write("Logical Name: \t {0}", ent.LogicalName);
            log.Write("Primary Key: \t {0}", ent.Id.ToString());
            log.Write(SEPERATOR);

            ent.Attributes.ToList().ForEach(a =>
            {
                log.Write("Key: \t {0}", a.Key);
                if (a.Value != null)
                {
                    log.Write("Type: \t {0}", a.Value.GetType());
                    if (a.Value is EntityReference) log.Write("Entity:\t {0}", ((EntityReference) a.Value).LogicalName);
                    log.Write("Value: \t {0}", GetFieldValue(a.Key, ent));
                }
                else
                {
                    log.Write("Type: \t {0}", "Value is Null");
                }

                log.Write(SEPERATOR);

            });

            log.Write("--- End of Entity ---");
            log.Write(SEPERATOR);
        }

        private static string GetEnum<T>(int p) where T : struct, IConvertible
        {
            return Enum.GetName(typeof (T), p);
        }

        private static string BoolToString(bool p)
        {
            if (p) return "True";
            else return "False";
        }

        private static bool IsInSandbox(IPluginExecutionContext Context)
        {
            return
                (Context.GetType().ToString().Equals("SandboxPluginExecutionContext", StringComparison.InvariantCulture));
        }

        public static string LogEntityReference(EntityReference entityReference)
        {
            return string.Format("Entity: \t {0} \t Id: {1} \t Name (if available): {2}", entityReference.LogicalName,
                entityReference.Id,
                !string.IsNullOrEmpty(entityReference.Name) ? entityReference.Name : "(NONE PROVIDED)");
        }

        public static string GetFieldValue(string attributeName, Entity ent)
        {
            if (!ent.Contains(attributeName)) return "(ATTRIBUTE NOT ON ENTITY)";
            if (ent[attributeName] == null) return "(ATTRIBUTE IS NULL)";

            var attr = ent[attributeName];

            if (attr is EntityReference) return ((EntityReference) attr).Id.ToString();
            else if (attr is OptionSetValue) return ((OptionSetValue) attr).Value.ToString();
            else return attr.ToString();
        }       

    }
}
