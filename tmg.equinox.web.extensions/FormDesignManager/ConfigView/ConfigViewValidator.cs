using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.web.FormDesignManager.ConfigView
{
    public class ConfigViewValidator
    {
        List<JToken> _elementList;

        public ConfigViewValidator(string elements)
        {
            _elementList = JToken.Parse(elements).ToList();
            int index = 0;
            foreach (var element in _elementList)
            {
                ((JObject)element).Add("index", index);
                index = index + 1;
            }
        }

        public List<int> Validate()
        {
            List<int> errorRows = new List<int>();
            var parentGroups = this._elementList.GroupBy(x => x["ParentUIElementId"]);
            foreach (var grouping in parentGroups)
            {
                var grpbylabel = grouping.GroupBy(s => s["Label"]);
                foreach (var grp in grpbylabel)
                {
                    if (grp.Count() > 1)
                    {
                        foreach (var row in grp)
                        {
                            errorRows.Add(Convert.ToInt32(row["index"]));
                        }
                    }
                }
            }

            return errorRows;
        }
    }
}
