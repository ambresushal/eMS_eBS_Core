using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.FormDesign;
using tmg.equinox.applicationservices.viewmodels.MasterList;
using tmg.equinox.applicationservices.viewmodels.UIElement;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IDataSourceService
    {
        #region DataSource Methods
        /// <summary>
        /// Get DataSources
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="formDesignVersionId"></param>
        /// <param name="uiElementId"></param>
        /// <returns></returns>
        IEnumerable<DataSourceRowModel> GeDataSourcesForUIElementType(int tenantId, int uiElementId, string uiElementType, int formDesignId, int formDesignVersionId);

        /// <summary>
        /// Get DataSource UIEklements
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="formDesignVersionId"></param>
        /// <param name="uiElementId"></param>
        /// <returns></returns>
        IEnumerable<UIElementRowModel> GetDataSourceUIElements(int tenantId, int uiElementId, string uiElementType, int dataSourceId, int formDesignId, int formDesignVersionId);

        /// <summary>
        /// Method to check if the Data Source Name is unique.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="formDesignVersionId"></param>
        /// <param name="dataSourceName"></param>
        /// <returns></returns>
        bool IsDataSourceNameUnique(int tenantId, int formDesignVersionId, string dataSourceName, int uiElementId, string uiElementType);

        /// <summary>
        /// Update DataSource UIElements
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="formDesignVersionId"></param>
        /// <param name="uiElementId"></param>
        /// <returns></returns>
        ServiceResult UpdateDataSource(List<DataSourceUiElementMappingModel> uiElementIds, int uiElementId, int tenantId, bool isEmptyDelete, string uiElementType, int formDesignId, int formDesignVersionId, List<int> existingDSIds,string userName);

        /// <summary>
        /// Add DataSource UIElements Mapping
        /// </summary>
        /// <param name="uiElementId"></param>
        /// <param name="dataSourceId"></param>
        /// <param name="mappedUIElementId"></param>
        /// <returns></returns>
        ServiceResult AddDataSourceUIElementMapping(int uiElementId, int dataSourceId, int mappedUIElementId);

        /// <summary>
        /// Get DataSourceElementDisplayModeModel
        /// </summary>
        /// <returns>List DataSourceElementDisplayModeModel </returns>
        IEnumerable<DataSourceElementDisplayModeModel> GetDataSourceElementDisplayMode();
        
         /// <summary>
        /// Get DataSourceDisplayModeModel
        /// </summary>
        /// <returns>List DataSourceDisplayModeModel </returns>
        IEnumerable<DataSourceModeViewModel> GetDataSourceDisplayMode();
        /// <summary>
        /// Get DataCopyMode
        /// </summary>
        /// <returns>List DataCopyMode</returns>
        IEnumerable<KeyValue> GetDataCopyMode();

        /// <summary>
        /// Update DataSource Display Mode
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="uiElementID"></param>
        /// <param name="displayModeID"></param>
        /// <returns></returns>
        ServiceResult UpdateDataSourceDisplayMode(int tenantID, int uiElementID, int displayModeID);


        IEnumerable<UIElementSeqModel> GetChildUIElementsForDataSource(int tenantId,int formDesignId, int formDesignVersionId, int parentUIElementId,bool isKey);

        #endregion

        bool GetDataSourceMappingExistence(int uiElementId, int formDesignId, int formDesignVersionId);

        IEnumerable<KeyValue> GetDataSourceFilterOperators();

    }
}
