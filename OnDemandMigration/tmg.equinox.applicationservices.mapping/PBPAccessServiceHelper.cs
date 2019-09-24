using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.pbp.dataaccess;

namespace tmg.equinox.applicationservices.pbp
{
    public static class PBPAccessServiceHelper
    {
        static internal JObject GetTableObject(PBPTable table)
        {
            List<JProperty> columns = new List<JProperty>();
            foreach (var col in table.ColumnNames.Distinct())
            {
                columns.Add(new JProperty(col, ""));
            }
            JObject columnObj = new JObject(columns);
            return columnObj;
        }


        static internal string GetQuery(PBPTable table, string whereClause, bool allColumns)
        {
            string query = "";
            string columns = String.Empty;
            if(allColumns == false)
            {
                foreach (var col in table.ColumnNames)
                {
                    columns = columns + "," + col;
                }
                columns = columns.TrimStart(',');
            }
            else
            {
                columns = "*";
            }
            if (columns != String.Empty)
            {
                query = String.Format(AccessFileConstants.SELECTQUERY, columns, table.TableName, whereClause);
            }
            return query;
        }
    }
}
