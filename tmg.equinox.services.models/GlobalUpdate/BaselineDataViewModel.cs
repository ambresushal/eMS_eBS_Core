using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.GlobalUpdate
{
    public class BaselineDataViewModel : ViewModelBase
    {
        public Guid batchId;
        public int tenantId;
        public int folderId;
        public int folderVersionId;
        public string versionNumber;
        public string comments;
        public System.DateTime effectiveDate;
        public bool isRelease;
    }
}
