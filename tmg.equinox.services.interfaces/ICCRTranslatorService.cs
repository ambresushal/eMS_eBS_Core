using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.CCRIntegration;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.domain.entities.Models.CCRTranslator;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface ICCRTranslatorService
    {
        /// <summary>
        /// This method is used to insert new product by adding products for traslation in the 'PluginVersionProcessQueueCommon' table
        /// </summary>
        /// <param name="producttoTranslate"></param>
        /// <returns></returns>        
        ServiceResult AddProducttoTranslate(FormInstanceViewModel formInstances, string currentuserName);

        string GetTableDetails(string id);

        List<TranslationQueueViewModel> GetTranslatorQueue();

        List<CCRTranslatorQueue> GetTranslatorProduct();

        string GetReportDetails(string id);
       
        DataSet GetTranslationLog(int processGovernance1up, int roleID);
    }
}
