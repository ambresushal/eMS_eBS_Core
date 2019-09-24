using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.pbp.dataaccess;

namespace tmg.equinox.applicationservices.pbp
{
    public class PBPDocumentService : IPBPDocumentService
    {
        public void UpdateDocumentJSON(int formInstanceId, string documentJSON)
        {
            using (eBenefitSyncModel model = new eBenefitSyncModel())
            {
                var formInstance = (from fi in model.FormInstanceDataMaps
                                    where fi.FormInstanceID == formInstanceId
                                    select fi).First();
                if (formInstance != null)
                {
                    formInstance.FormData = documentJSON;
                    model.SaveChanges();
                }
            }
        }

        public JObject GetJSONDocument(int formInstanceId)
        {
            JObject defaultJSON = new JObject();
            using (eBenefitSyncModel model = new eBenefitSyncModel())
            {
                var formInstance = (from fi in model.FormInstanceDataMaps
                                    where fi.FormInstanceID == formInstanceId
                                    select fi).First();
                if (formInstance != null)
                {
                    defaultJSON = JObject.Parse(formInstance.FormData);
                }
            }
            return defaultJSON;
        }
    }
}
