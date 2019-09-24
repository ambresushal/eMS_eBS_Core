using System;
using System.Collections.Generic;


namespace tmg.equinox.domain.entities.Models
{
    public class AccessFiles : Entity
    {
        public int FileID { get; set; }
        public int BatchId { get; set; }
        public string FileName { get; set; }
        public string OriginalFileName { get; set; }
        public virtual MigrationBatchs MigrationBatch { get; set; }
        public virtual ICollection<MigrationPlans> MigrationPlans { get; set; }

    }
}
