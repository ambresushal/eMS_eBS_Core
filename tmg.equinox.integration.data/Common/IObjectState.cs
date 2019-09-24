using System.ComponentModel.DataAnnotations.Schema;

namespace tmg.equinox.integration.data
{
    public interface IObjectState
    {
        [NotMapped]
        ObjectState ObjectState { get; set; }
    }
}
