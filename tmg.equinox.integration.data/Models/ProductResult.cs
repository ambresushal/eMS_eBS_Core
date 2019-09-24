using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.data.Models
{
    public class ProductResult : Entity
    {
        public int ProcessGovernance1Up { get; set; }
        public string Product { get; set; }
        public string FolderVersionNumber { get; set; }
        public string AccountName { get; set; }
        public string FolderName { get; set; }
        public string PDDS_DESC { get; set; }
        public DateTime EffectiveDate { get; set; }        
        public int ProductID { get; set; }
    }
}
