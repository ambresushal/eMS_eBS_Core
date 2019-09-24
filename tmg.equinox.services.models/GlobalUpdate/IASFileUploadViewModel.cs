using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.GlobalUpdate
{
    public class IASFileUploadViewModel : ViewModelBase
    {
        #region Instance Properties
        public int IASFileUploadID { get; set; }
        public int GlobalUpdateID { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public string TemplateGuid { get; set; }
        #endregion Instance Properties
    }
}
