using System.ComponentModel;
namespace tmg.equinox.domain.entities.Enums
{
    public enum ExceptionMessages
    {
        [Description("Unable to Delete finalized Form Design Version")]
        FINALIZEDVERSION,
        [Description("Data source is already in use. You can not delete it")]
        USEDDATASOURCE,
        [Description("Data source is associated with Finalized FormDesignVersion. You can not delete it.")]
        USEDDATASOURCEINFINALIZEDFORM,
        [Description("")]
        NULL
    }
}
