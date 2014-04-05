using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ThinkCrmPlugin.Core.Logging
{
    class NonSandboxedExceptionLogging
    {
        //From the example at http://stackoverflow.com/a/12827271/394978
        /// <summary>
        /// This utility method can be used for retrieving extra details from exception objects.
        /// </summary>
        /// <param name="e">Exception.</param>
        /// <param name="indent">Optional parameter. String used for text indent.</param>
        /// <returns>String with as much details was possible to get from exception.</returns>
        public static string GetExtendedExceptionDetails(object e, string indent = null)
        {
            // we want to be robust when dealing with errors logging
            try
            {
                var sb = new StringBuilder(indent);
                sb.AppendLine("NonSandboxedExceptionLogging");
                // it's good to know the type of exception
                sb.AppendLine("Type: " + e.GetType().FullName);
                // fetch instance level properties that we can read
                var props = e.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanRead);

                foreach (PropertyInfo p in props)
                {
                    try
                    {
                        var v = p.GetValue(e, null);

                        // in case of Fault contracts we'd like to know what Detail contains
                        if (e is FaultException && p.Name == "Detail")
                        {
                            sb.AppendLine(string.Format("{0}{1}:", indent, p.Name));
                            sb.AppendLine(GetExtendedExceptionDetails(v, "  " + indent));// recursive call
                        }
                        // Usually this is InnerException
                        else if (v is Exception)
                        {
                            sb.AppendLine(string.Format("{0}{1}:", indent, p.Name));
                            sb.AppendLine(GetExtendedExceptionDetails(v as Exception, "  " + indent));// recursive call
                        }
                        // some other property
                        else
                        {
                            sb.AppendLine(string.Format("{0}{1}: '{2}'", indent, p.Name, v));

                            // Usually this is Data property
                            if (v is IDictionary)
                            {
                                var d = v as IDictionary;
                                sb.AppendLine(string.Format("{0}{1}={2}", " " + indent, "count", d.Count));
                                foreach (DictionaryEntry kvp in d)
                                {
                                    sb.AppendLine(string.Format("{0}[{1}]:[{2}]", " " + indent, kvp.Key, kvp.Value));
                                }
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        sb.AppendLine(string.Format("GetExtendedExceptionDetails: Exception during logging of property: {0}", exception));
                    }
                }

                //remove redundant CR+LF in the end of buffer
                sb.Length = sb.Length - 2;
                return sb.ToString();
            }
            catch (Exception exception)
            {
                //log or swallow here
                return string.Format("GetExtendedExceptionDetails: Exception during logging of Exception message: {0}", exception);
            }
        }
    }
}
