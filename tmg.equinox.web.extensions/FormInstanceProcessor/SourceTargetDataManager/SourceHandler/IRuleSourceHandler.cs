using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.web.sourcehandler
{
   public  interface IRuleSourceHandler
    {
        Dictionary<string, JToken> GetSourceData();
    }
}
