using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class IASWizardStep : Entity
    {
        public IASWizardStep()
        {
            this.GlobalUpdates = new List<GlobalUpdate>();
        }
        

        public int IASWizardStepID { get; set; }
        public string Name { get; set; }
        public string NameID { get; set; }
        public bool IsActive { get; set; }

        public ICollection<GlobalUpdate> GlobalUpdates { get; set; }
    }
    
}
