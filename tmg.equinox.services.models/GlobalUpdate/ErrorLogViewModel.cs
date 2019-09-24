using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.GlobalUpdate
{
    public class ErrorLogViewModel
    {
        #region Instance Properties

        public int ErrorLogID { get; set; }
        public int GlobalUpdateID { get; set; }
        public int IASElementExportID { get; set; }
        public int IASFolderMapID { get; set; }
        public string AccountName { get; set; }
        public int FolderID { get; set; }
        public string FolderName { get; set; }
        public int FolderVersionID { get; set; }
        public string FolderVersionNumber { get; set; }
        public DateTime EffectiveDate { get; set; }
        public int FormInstanceID { get; set; }
        public string FormName { get; set; }
        public string Owner { get; set; }
        public int FormDesignID { get; set; }
        public string Label { get; set; }
        public string ElementFullPath { get; set; }
        public string RuleErrorDescription { get; set; }
        public string DataSourceErrorDescription { get; set; }
        public string ValidationErrorDescription { get; set; }
        public string AddedBy { get; set; }
        public DateTime AddedDate { get; set; }

        #endregion Instance Properties
    }
}
