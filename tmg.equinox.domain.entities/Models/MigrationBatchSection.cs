using System;
using System.Collections.Generic;


namespace tmg.equinox.domain.entities.Models
{
    public class MigrationBatchSection : Entity
    {
        public int ID { get; set; }
        public int BatchId { get; set; }
        public string SectionGeneratedName { get; set; }

        public string ViewType { get; set; }
        public virtual MigrationBatchs MigrationBatch { get; set; }

    }
}
