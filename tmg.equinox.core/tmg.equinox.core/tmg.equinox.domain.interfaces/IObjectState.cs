using System.ComponentModel.DataAnnotations.Schema;

namespace tmg.equinox.repository.interfaces
{
    public interface IObjectState
    {
        [NotMapped]
        ObjectState ObjectState { get; set; }
    }
}
