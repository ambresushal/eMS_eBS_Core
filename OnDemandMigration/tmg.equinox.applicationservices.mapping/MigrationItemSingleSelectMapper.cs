using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.pbp
{
    public class MigrationItemSingleSelectMapper : IMigrationItemMapper
    {
        public void MapItem(ref JObject source, ref JObject target, MigrationFieldItem item, MigrationPlanItem plan)
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

                string logMessage = item.TableName + "," + item.ColumnName + "," + item.DocumentPath + "," + prop.Value.ToString().Replace(",", ";");
                logMessage = Regex.Replace(logMessage, @"\t|\n|\r", "");
                DMUtilityLog.WriteDMUtilityLog(plan.QID, item.ViewType, logMessage);
            }
        }
    }
}
