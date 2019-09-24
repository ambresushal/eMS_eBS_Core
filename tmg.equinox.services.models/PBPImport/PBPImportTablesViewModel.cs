using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.PBPImport
{
    public class PBPImportTablesViewModel
    {
        public int PBPTableID { get; set; }
        public string PBPTableName { get; set; }
        public int PBPTableSequence { get; set; }
        public string EBSTableName { get; set; }

        //public List<PBPImportTableColumnsViewModel> PBPImportTableColumnsViewModel { get; set; }
    }
}
