using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace tmg.equinox.web.CustomRule
{
    public interface ICustomRuleAssociatedSectionHandler
    {
        List<string> getAssociatedSections(string sectionName);        
    }
}
