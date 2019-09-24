using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Newtonsoft.Json;
using tmg.equinox.applicationservices;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.PBPImport;
using tmg.equinox.dependencyresolution;
using Newtonsoft.Json.Linq;


namespace tmg.equinox.pbpimportservices
{
    public class ImportDatatoFolder
    {
        int _formInstanceID;
        string _QID;
        int _pbpImportBatchId;
        IPBPImportServices _pbpImportService;
        IFolderVersionServices _folderVersionServices;

        public ImportDatatoFolder(int pbpImportBatchId, string QID, int formInstanceID)
        {
            UnityConfig.RegisterComponents();
            _pbpImportService = UnityConfig.Resolve<IPBPImportServices>();
            _folderVersionServices = UnityConfig.Resolve<IFolderVersionServices>();

            _formInstanceID = formInstanceID;
            _QID = QID;
            _pbpImportBatchId = pbpImportBatchId;

            //_formInstanceID = 65293;
            //_QID = "H8578015000";
            //_pbpImportBatchId = 1;
        }

        public void UpdatePBPView()
        {   
            string PBPJson = _folderVersionServices.GetFormInstanceDataCompressed(1, _formInstanceID);

            JObject jObject = new JObject();
            if(!string.IsNullOrEmpty(PBPJson))
                jObject = JObject.Parse(PBPJson);

            List<PBPDataMapViewModel>  pbpDataModelList = _pbpImportService.GetPBPDataMapList(_pbpImportBatchId).ToList();
            List<PBPDataMapViewModel> pbplist = pbpDataModelList.Where(p => p.QID == _QID).ToList();
            List<string> tablename = pbplist.Select(t => t.TableName).Distinct().ToList();

            foreach (string name in tablename)
            {
                List<PBPDataMapViewModel> pbps = pbplist.Where(p => p.TableName == name).ToList();
                if (pbps != null && pbps.Count == 1)
                {
                    var abc = JObject.Parse(pbps[0].JsonData);
                    jObject[name] = abc as JToken;
                }
                else if (pbps != null && pbps.Count > 1)
                {
                    JArray list = new JArray();
                    foreach (PBPDataMapViewModel p in pbps)
                    {
                        var abc = JObject.Parse(p.JsonData);
                        list.Add(abc);
                    }
                    if (jObject[name] != null)
                        jObject[name]["RPT" + name] = list;
                    else
                    {
                        JObject j = new JObject();
                        j["RPT" + name] = list;
                        jObject[name] = j;
                    }
                }
            }

            string json1 = JsonConvert.SerializeObject(jObject);
            _folderVersionServices.SaveFormInstanceDataCompressed(_formInstanceID, json1);
        }
    }
}
