using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class BatchIASMap : Entity
    {
        public BatchIASMap()
        {
        }

        #region Instance Properties

        public int BatchIASMapID { get; set; }

        public Guid BatchID { get; set; }

        public int GlobalUpdateID { get; set; }

        public virtual Batch Batch { get; set; }
        public virtual GlobalUpdate GlobalUpdate { get; set; }

        #endregion Instance Properties
    }
}
