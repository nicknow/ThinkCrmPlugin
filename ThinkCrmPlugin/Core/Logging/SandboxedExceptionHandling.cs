using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ThinkCrmPlugin.Core.Logging
{
    public class SandboxedExceptionHandling
    {
        /// <summary>
        /// This utility method can be used for retrieving details of exeception objects when Reflection is prohibited by Sandbox.
        /// </summary>
        /// <param name="e">Exception.</param>        
        public static string GetExtendEdexceptionDetails(object e, string indent = null)
        {
            try
            {
                var sb = new StringBuilder(indent);
                sb.AppendLine("SandboxedExceptionHandling");
                sb.AppendLine("Type: " + e.GetType().FullName);

                if (e is FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault>)
                {
                    var ex = e as FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault>;
                    sb.AppendFormat("FaultException").AppendLine();
                    sb.AppendFormat("Timestamp: {0}", ex.Detail.Timestamp).AppendLine();
                    sb.AppendFormat("Code: {0}", ex.Detail.ErrorCode).AppendLine();
                    sb.AppendFormat("Message: {0}", ex.Detail.Message).AppendLine();
                    sb.AppendFormat("Inner Fault: {0}",
                        ex.Detail.InnerFault != null ? ex.Detail.InnerFault.Message : "(No Inner Exception)").AppendLine();
                    if (ex.Detail.InnerFault != null)
                        sb.Append(GetExtendEdexceptionDetails(ex.Detail.InnerFault, "  " + indent)).AppendLine();

                }
                else if (e is TimeoutException)
                {
                    var ex = e as TimeoutException;
                    sb.AppendFormat("Timeout Exception").AppendLine();
                    sb.AppendFormat("Message: {0}", ex.Message).AppendLine();
                    sb.AppendFormat("Stack Trace: {0}", ex.StackTrace).AppendLine();
                    sb.AppendFormat("Inner Fault: {0}",
                        ex.InnerException != null ? ex.InnerException.Message : "(No Inner Exception)").AppendLine();
                    if (ex.InnerException != null)
                        sb.Append(GetExtendEdexceptionDetails(ex.InnerException, "  " + indent)).AppendLine();
                }
                else
                {
                    var ex = e as Exception;
                    sb.AppendFormat("Exception.").AppendLine();
                    sb.AppendFormat("Type: {0}", ex.GetType().Name);
                    sb.AppendFormat("Message: {0}", ex.Message).AppendLine();
                    sb.AppendFormat("Stack Trace: {0}", ex.StackTrace).AppendLine();
                    sb.AppendFormat("Inner Fault: {0}",
                                        ex.InnerException != null ? ex.InnerException.Message : "(No Inner Exception)").AppendLine();
                    if (ex.InnerException != null)
                        sb.Append(GetExtendEdexceptionDetails(ex.InnerException, "  " + indent)).AppendLine();
                }

                return sb.ToString();
            }
            catch (Exception exception)
            {
                //log or swallow here
                return string.Format("GetExtendedExceptionDetails: Exception during logging of Exception message: {0}", exception.ToString());
            }
        }
    }
}

