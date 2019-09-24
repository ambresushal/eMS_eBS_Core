using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.web.ODMExecuteManager.Interface;
using tmg.equinox.web.ODMExecuteManager.Model;

namespace tmg.equinox.web.ODMExecuteManager
{

    public class MigrationItemSingleSelectMapper : IMigrationItemMapper
    {
        public void MapItem(ref JObject source, ref JObject target, MigrationFieldItem item)
        {

            string sourcePath = item.TableName + "." + item.ColumnName.Replace(" ", "");
            string targetPath = item.DocumentPath;
            var sourceToken = source.SelectToken(sourcePath);

            if (item.DocumentPath == "SectionA.SectionA1.OrganizationType" || item.DocumentPath == "SectionA.SectionA1.PlanType")
                if (sourceToken != null && sourceToken.ToString() != "")
                    sourceToken = Convert.ToInt32(sourceToken).ToString();

            var targetToken = target.SelectToken(targetPath);
            if (targetToken != null)
            {
                var prop = targetToken.Parent as JProperty;
                prop.Value = sourceToken != null ? sourceToken.ToString().Trim() : "";
            }
        }

    }

}
