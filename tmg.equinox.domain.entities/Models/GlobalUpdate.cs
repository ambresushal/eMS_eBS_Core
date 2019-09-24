using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class GlobalUpdate : Entity
    {
        public GlobalUpdate()
        {
            this.FormDesignElementValues = new List<FormDesignElementValue>();
            this.IASElementExports = new List<IASElementExport>();
            //this.Batches = new List<Batch>();
            this.ErrorLogs = new List<ErrorLog>();
            this.IASElementImports = new List<IASElementImport>();
            this.IASFileUploads = new List<IASFileUpload>();
            this.IASFolderMaps = new List<IASFolderMap>();
            this.RulesGu = new List<RuleGu>();
            this.BatchIASMaps = new List<BatchIASMap>();
        }

        public int GlobalUpdateID { get; set; }
        public int TenantID { get; set; }
        public int GlobalUpdateStatusID { get; set; }
        public int IASWizardStepID { get; set; }
        public string GlobalUpdateName { get; set; }
        public DateTime EffectiveDateFrom { get; set; }
        public DateTime EffectiveDateTo { get; set; }
        public bool IsActive { get; set; }
        public bool IsIASDownloaded { get; set; }
        public bool IsErrorLogDownloaded { get; set; }
        public DateTime AddedDate { get; set; }
        public string AddedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public virtual ICollection<FormDesignElementValue> FormDesignElementValues { get; set; }
        public virtual ICollection<IASElementExport> IASElementExports { get; set; }
        public virtual Tenant Tenant { get; set; }
        public virtual GlobalUpdateStatus GlobalUpdateStatus { get; set; }
        //public virtual ICollection<Batch> Batches { get; set; }
        public virtual ICollection<ErrorLog> ErrorLogs { get; set; }
        public virtual ICollection<IASElementImport> IASElementImports { get; set; }
        public virtual ICollection<IASFileUpload> IASFileUploads { get; set; }
        public virtual ICollection<IASFolderMap> IASFolderMaps { get; set; }
        public virtual ICollection<BatchIASMap> BatchIASMaps { get; set; }
        public IASWizardStep iasWizardStep { get; set; }
        public virtual ICollection<RuleGu> RulesGu { get; set; }
    }
}
