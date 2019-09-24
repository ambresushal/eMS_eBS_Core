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
using tmg.equinox.applicationservices;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.dependencyresolution;

namespace tmg.equinox.pbpimportservices
{
    public class PBPObjectJson
    {
        private IPBPImportServices _pbpImportService;
        int _pbpImportBatchId;

        public PBPObjectJson(int pbpImportBatchId)
        {
            UnityConfig.RegisterComponents();

            _pbpImportService = UnityConfig.Resolve<IPBPImportServices>();            
            this._pbpImportBatchId = pbpImportBatchId;
        }

        public void GenerateJsonForBatchId()
        {
            _pbpImportService.GenerateJsonForBatchId(this._pbpImportBatchId);
        }
    }
}
