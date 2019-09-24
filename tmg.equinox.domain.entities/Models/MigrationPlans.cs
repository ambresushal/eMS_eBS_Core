using System;
using System.Collections.Generic;


namespace tmg.equinox.domain.entities.Models
{
    public class MigrationPlans : Entity
    {
        public int MigrationPlanID { get; set; }
        public int BatchId { get; set; }
        public int FileID { get; set; }
        public int FolderId { get; set; }

        public int FolderVersionId { get; set; }

        public int FormInstanceId { get; set; }

        public int FormDesignVersionId { get; set; }

        public string ViewType { get; set; }

        public string QID { get; set; }

        public virtual MigrationBatchs MigrationBatch { get; set; }

        public virtual AccessFiles AccessFiles { get; set; }
    }
}
