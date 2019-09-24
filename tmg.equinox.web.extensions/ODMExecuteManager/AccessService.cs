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
    public class AccessService : IAccessService
    {

        public JObject GetQIDBenefitData(string QID, string mdbFilePath)
        {
            AccessDbContext context = new AccessDbContext(mdbFilePath);
            throw new NotImplementedException();

        }

        public List<string> GetQIDList(string mdbFilePath)
        {
            AccessDbContext context = new AccessDbContext(mdbFilePath);
            string selectQuery = String.Format(AccessFileConstants.SELECTQUERY, AccessFileConstants.QIDCOLUMN, AccessFileConstants.QIDTABLE, "1 = 1");
            return context.GetQIDList(selectQuery);
        }

        public JObject GetQIDData(string mdbFilePath, List<TableInfo> tables, string QID)
        {
            JObject qidInstance = new JObject();
            foreach (var table in tables)
            {

                JObject tableObject = AccessServiceHelper.GetTableObject(table);
                string query = AccessServiceHelper.GetQuery(table, "QID = '" + QID + "'", true);
                AccessDbContext context = new AccessDbContext(mdbFilePath);

                if (domain.entities.GlobalVariables.OptionalSupplementalPackageTables.Contains(table.TableName) || table.TableName == "PBPC_POS" || table.TableName == "PBPD_OPT")
                    context.GetQIDData(query, ref tableObject, true);
                else
                    context.GetQIDData(query, ref tableObject, false);

                if (tableObject[""] != null)
                {
                    qidInstance.Add(table.TableName, tableObject[""]);
                }
                else
                {
                    qidInstance.Add(table.TableName, tableObject);
                }
            }
            return qidInstance;
        }
    }

}
