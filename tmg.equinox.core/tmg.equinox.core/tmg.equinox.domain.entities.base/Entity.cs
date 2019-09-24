using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.domain.entities
{
    /// <summary>
    /// provides abstraction for common attributes for entities
    /// </summary>
    public abstract class Entity:IObjectState
    {
         [NotMapped]
        public virtual int Id
        {
            get;
            set;
        }

        //public string AddedBy { get; set; }
        
        //public System.DateTime? AddedDate { get; set; }
        
        //public string UpdatedBy { get; set; }
        
        //public System.DateTime? UpdatedDate { get; set; }

        [NotMapped]
        public ObjectState ObjectState { get; set; }

    }
}
