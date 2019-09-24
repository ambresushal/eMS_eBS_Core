using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.Qhp
{
    public class QhpPackageViewModel : ViewModelBase
    {
        #region Public Properties
        public string AccountName { get; set; }
        public string FolderName { get; set; }
        public string FolderVersion { get; set; }
        #endregion Public Properties

        #region Constructor
        public QhpPackageViewModel()
        {

        }
        #endregion Constructor
    }
}
