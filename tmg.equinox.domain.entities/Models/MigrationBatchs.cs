using System;
using System.Collections.Generic;


namespace tmg.equinox.domain.entities.Models
{
    public class MigrationBatchs : Entity
    {
        public int BatchId { get; set; }
        public string Description { get; set; }
 
        public DateTime QueuedDate { get; set; }

        public string QueuedUser { get; set; }

        public DateTime? ExecutedDate { get; set; }

        public string Status { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<AccessFiles> AccessFilesMap { get; set; }

        public virtual ICollection<MigrationBatchSection> MigrationBatchSection { get; set; }

        public virtual ICollection<MigrationPlans> MigrationPlans { get; set; }

    }
}
