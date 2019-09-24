namespace tmg.equinox.applicationservices.interfaces
{
    public interface IDataValueService
    {
        ServiceResult SaveDataValuesFromFolderVersion(int tenantId, int folderVersionId, int formInstanceId,
                                                      string formInstanceData);

        ServiceResult GetDeserializedJsonObject(dynamic jsonObject, int formDesignVersionId, int tenantId);
    }
}
