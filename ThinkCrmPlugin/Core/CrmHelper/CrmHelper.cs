using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkCrmPlugin.Core.Interfaces;


namespace ThinkCrmPlugin.Modifiable
{
    public partial class CrmHelper : ICrmHelper
    {
        private readonly ICrmService _crmService;
        private readonly ILogging _logging;
        private readonly ICrmResourceStrings _crmResourceStrings;

        public CrmHelper(ICrmService crmService, ILogging logging, ICrmResourceStrings crmResourceStrings)
        {
            if (crmService == null) throw new ArgumentNullException("crmService");
            if (logging == null) throw new ArgumentNullException("logging");
            _crmService = crmService;
            _logging = logging;
            _crmResourceStrings = crmResourceStrings;
            
            _logging.Write(GetString("LoggingCrmHelperCreated", "Created CrmHelper"));
        }

        private string GetString(string stringId, string defaultValue)
        {
            return _crmResourceStrings != null ? _crmResourceStrings.GetString(stringId, defaultValue) : defaultValue;
        } 

    }
}
