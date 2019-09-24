namespace tmg.equinox.pbp.dataaccess
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Fldr.FormInstanceDataMap")]
    public partial class FormInstanceDataMap
    {
        public int FormInstanceDataMapID { get; set; }

        public int FormInstanceID { get; set; }

        public int ObjectInstanceID { get; set; }

        public string FormData { get; set; }

        public string CompressJsonData { get; set; }

        public virtual FormInstance FormInstance { get; set; }
    }
}
