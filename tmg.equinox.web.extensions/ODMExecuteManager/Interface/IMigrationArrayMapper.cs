using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.web.ODMExecuteManager.Model;

namespace tmg.equinox.web.ODMExecuteManager.Interface
{
    public interface IMigrationArrayMapper
    {
        void MapArray(ref JObject source, ref JObject target, List<MigrationFieldItem> items);
    }
}
