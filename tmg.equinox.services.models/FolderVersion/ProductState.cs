using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.FolderVersion
{
    public class ProductState
    {
        public int FormInstanceID { get; set; }
        public bool IsProductInTranslation { get; set; }
        public bool IsProductInTransmission { get; set; }
        public bool IsFolderVersionReleased { get; set; }
        public bool IsFolderVersionBaselined { get; set; }

        public bool IsProductInMigration { get; set; }
    }
}
