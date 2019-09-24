using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class IASElementExport : Entity
    {
        public IASElementExport()
        {
            this.ErrorLogs = new List<ErrorLog>();
        }

        public int IASElementExportID { get; set; }
        public int GlobalUpdateID { get; set; }
        public int IASFolderMapID { get; set; }
        public int UIElementID { get; set; }
        public int UIElementTypeID { get; set; }
        public string ElementHeaderName { get; set; }
        public string UIElementName { get; set; }
        public string NewValue { get; set; }
        public string OldValue { get; set; }
        public bool AcceptChange { get; set; }
        public DateTime AddedDate { get; set; }
        public string AddedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public virtual ICollection<ErrorLog> ErrorLogs { get; set; }
        public virtual GlobalUpdate GlobalUpdate { get; set; }
        public virtual UIElement UIElement { get; set; }
        public virtual UIElementType UIElementType { get; set; }
        public virtual IASFolderMap IASFolderMap { get; set; }
    }
}
