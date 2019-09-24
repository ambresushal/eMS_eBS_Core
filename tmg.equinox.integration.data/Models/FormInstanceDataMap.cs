
namespace tmg.equinox.integration.data.Models
{
    public partial class FormInstanceDataMap : Entity
    {
        public int FormInstanceDataMapID { get; set; }
        public int FormInstanceID { get; set; }
        public int ObjectInstanceID { get; set; }
        public string FormData { get; set; }
        public string CompressJsonData { get; set; } 
        public virtual FormInstance FormInstance { get; set; }
    }
}
