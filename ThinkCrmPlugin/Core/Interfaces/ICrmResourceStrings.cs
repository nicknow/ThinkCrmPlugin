using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkCrmPlugin.Modifiable;

namespace ThinkCrmPlugin.Core.Interfaces
{
    public interface ICrmResourceStrings
    {
        string GetString(string stringId);
        string GetString(string stringId, string defaultValue);
    }
}
