using System.ComponentModel.DataAnnotations.Schema;


namespace tmg.equinox.integration.data
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
