using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;

namespace tmg.equinox.web.FormInstanceProcessor
{
    public interface IDataProcessor
    {
        void RunProcessorOnDocumentLoad(string sectionName, bool isRefreshCache);

        void RunProcessorOnSectionLoad(string sectionName, bool isRefreshCache);

        void RunProcessorOnSectionSave(string sectionName, bool isRefreshCache, string currentSectionData, string previousSectionData);

        void RunProcessorOnDocumentSave(string sectionName, bool isRefreshCache, string currentSectionData, string previousSectionData);

        void RunViewProcessorsOnCollateralGeneration();

        void RunViewProcessorsOnAnchorSave();

        void ExecuteProcessorOnDocumentLoad();

        ServiceResult UpdateAccountProductMap(int folderId, int folderVersionId, IConsumerAccountService consumerAccountService, ServiceResult result);

        ServiceResult UpdatePBPDetails(int folderId, int folderVersionId, string planName, string planNumber, IConsumerAccountService consumerAccountService, ServiceResult result);

        void RunPreExitValidateRules(string sectionName, bool isRefreshCache);

        bool RunSectionVisibleRule(string sectionName);
    }
}
