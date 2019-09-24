using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class AutoSaveSettings : Entity
    {
        public int  SettingsID  { get; set; }
        public int  UserID      { get; set; }
        public bool IsAutoSaveEnabled{ get; set; }
        public int  Duration    { get; set; }
        public int  TenantID    { get; set; }
        public DateTime AddedDate   { get; set; }
        public string  AddedBy     { get; set; }
        public DateTime?    UpdatedDate  { get; set; }
        public string UpdatedBy { get; set; }
        public virtual Tenant Tenant { get; set; }
        public virtual User User { get; set; }
    }
}
