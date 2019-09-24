using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.pbpimportservices.PBPMappingsBuilder
{
    public class NetworkBuilder
    {
        MapPBPData _mapPBPData;
        public NetworkBuilder(MapPBPData mapPBPData)
        {
            this._mapPBPData = mapPBPData;
        }

        public void PopulateNetwork(JObject json)
        {
            JArray jarry = new JArray();

            JObject j = new JObject();
            j[ELEMENTNAME.CostShareTiers] = DATA.InNetwork;

            jarry.Add(j);


            if (_mapPBPData._isOONApplicable)
            {
                j = new JObject();
                j[ELEMENTNAME.CostShareTiers] = DATA.OutOfNetwork;
                jarry.Add(j);
            }

            json.SelectToken(SECTIONNAME.Tiers)[REPEATERNAME.SelectthePlansCostShareTiers] = jarry;
        }
    }
}
