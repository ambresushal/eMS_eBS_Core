using System.ComponentModel.DataAnnotations.Schema;

namespace tmg.equinox.domain.entities
{
    public interface IObjectState
    {
        [NotMapped]
        ObjectState ObjectState { get; set; }
    }
}
