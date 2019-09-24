using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities;

namespace tmg.equinox.domain.entities.Models
{
    public class CollateralProcessUpload : Entity
    {
        public int ID { get; set; }
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        public int? AccountID { get; set; }
        public string AccountName { get; set; }
        public int? FolderID { get; set; }
        public string FolderName { get; set; }
        public int? FolderVersionID { get; set; }
        public string FolderVersionNumber { get; set; }
        public int? FormInstanceID { get; set; }
        public string FormInstanceName { get; set; }
        public int? FormDesignID { get; set; }
        public int? FormDesignVersionID { get; set; }
        public byte[] WordFile { get; set; }
        public byte[] PrintxFile { get; set; }
        public byte[] File508 { get; set; }
        public bool? HasError { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ErrorDescription { get; set; }
        public string CollateralName { get; set; }
    }
}
