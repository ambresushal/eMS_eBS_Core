using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.MasterList;

namespace tmg.equinox.web.masterlist
{
    public class AliasListGenerator
    {
        JObject _data;
        public AliasListGenerator(JObject data)
        {
            _data = data;
        }

        public List<MasterListAliasViewModel> GetAliases()
        {
            List<MasterListAliasViewModel> aliases = new List<MasterListAliasViewModel>();
            foreach(var token in _data)
            {
                JToken aliasList = _data.SelectToken(token.Key + "." + token.Key + "AliasList");
                foreach(var alias in aliasList)
                {
                    MasterListAliasViewModel model = new MasterListAliasViewModel();
                    model.Alias = alias["Alias"].ToString();
                    model.DesignName = token.Key;
                    model.FullName = model.DesignName + "[" + model.Alias + "]";
                    model.ElementType = alias["ElementType"].ToString();
                    model.Items = alias["Items"].ToString();
                    aliases.Add(model);
                }
            }
            return aliases.Distinct(new AliasEqualityComparer()).ToList();
        }
    }

    class AliasEqualityComparer : IEqualityComparer<MasterListAliasViewModel>
    {
        public bool Equals(MasterListAliasViewModel b1, MasterListAliasViewModel b2)
        {
            if (b2 == null && b1 == null)
                return true;
            else if (b1 == null | b2 == null)
                return false;
            else if (b1.FullName == b2.FullName)
                return true;
            else
                return false;
        }

        public int GetHashCode(MasterListAliasViewModel bx)
        {
            int hCode = bx.FullName.GetHashCode();
            return hCode.GetHashCode();
        }
    }


}
