using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.web.DataSource;
using tmg.equinox.web.FormInstanceManager;

namespace tmg.equinox.web.FormInstanceProcessor
{
    public class DataSourceProcessor
    {
        private int _tenantId = 1;
        private int _formInstanceId;
        private int _folderVersionId;
        private int _formDesignVersionId;
        private bool _isFolderReleased;
        private string _sectionName;
        private FormDesignVersionDetail _detail;
        private IFolderVersionServices _folderVersionServices;
        private FormInstanceDataManager _formDataInstanceManager;
        private IFormDesignService _formDesignServices;

        public DataSourceProcessor(int tenantId, int formInstanceId, int folderVersionId, FormDesignVersionDetail detail, int formDesignVersionId, bool isFolderReleased, IFolderVersionServices folderVersionServices, FormInstanceDataManager formDataInstanceManager, string sectionName, IFormDesignService formDesignServices)
        {
            this._tenantId = tenantId;
            this._formInstanceId = formInstanceId;
            this._folderVersionId = folderVersionId;
            this._formDesignVersionId = formDesignVersionId;
            this._isFolderReleased = isFolderReleased;
            this._sectionName = sectionName;
            this._detail = detail;
            this._folderVersionServices = folderVersionServices;
            this._formDataInstanceManager = formDataInstanceManager;
            this._formDesignServices = formDesignServices;
        }

        public void Run()
        {
            MapDataSource();            
        }

        private void MapDataSource()
        {
            DataSourceMapper dm = new DataSourceMapper(_tenantId, _formInstanceId, _folderVersionId, _detail.FormDesignId, _formDesignVersionId, _isFolderReleased, _folderVersionServices, _detail.JSONData, _detail, _formDataInstanceManager, _sectionName,this._formDesignServices);
            dm.AddDataSourceRange(_detail.DataSources);
            _detail.JSONData = dm.MapDataSources();
        }
    }
}