using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.GlobalUpdate;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IIASDocumentService
    {
        List<IASElementExportDataModel> GetGlobalUpdatesDataList(int GlobalUpdateID);
        ServiceResult ExportIASExcelTemplate(int GlobalUpdateID, string GlobalUpdateName, DateTime GlobalUpdateEffectiveDateFrom, DateTime GlobalUpdateEffectiveDateTo, string folderPath, out string filePath);
        List<FormDesignElementValueVeiwModel> GetGlobalUpdatesDesignDocumentList(int GlobalUpdateID);
        ServiceResult ValidateIASExcelTemplate(int GlobalUpdateID, string GlobalUpdateName, DateTime GlobalUpdateEffectiveDateFrom, DateTime GlobalUpdateEffectiveDateTo, string importPath, string addedBy, string folderPath, out string filePath);
        void UpdateGlobalUpdateIASStatus(int GlobalUpdateID, int flag);
    }
}
