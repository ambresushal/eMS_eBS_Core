using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    /// <summary>
    /// provides abstraction for common attributes for entities
    /// </summary>
    public abstract class Entity
    {
        public virtual int Id
        {
            get;
            set;
        }

        public string AddedBy { get; set; }
        
        public System.DateTime? AddedDate { get; set; }
        
        public string UpdatedBy { get; set; }
        
        public System.DateTime? UpdatedDate { get; set; }

    }
}
