using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.pbp
{
    public interface IMigrationItemMapper
    {
        void MapItem(ref JObject source, ref JObject target, MigrationFieldItem item, MigrationPlanItem plan);

    }
}
