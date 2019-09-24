using System;
using System.Collections.Generic;
using tmg.equinox.applicationservices.interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.caching;
using tmg.equinox.infrastructure.util;
using tmg.equinox.ruleprocessor.formdesignmanager;
using tmg.equinox.expressionbuilder;
using tmg.equinox.core.logging.Logging;

namespace tmg.equinox.ruleprocessor
{
    public class FormInstanceDataManager
    {
        private IFormInstanceDataServices _formInstanceDataServices;
        private FormInstanceSectionDataCacheHandler _handler;
        private IFolderVersionServices _folderVersionServices;
        private IFormDesignService _formDesignServices;
        private int _tenantId;
        private int? _userId;
        private string _currentUserName;
        private static object lockThis = new object();
        private bool _loadAllSections = false;
        private static readonly ILog _logger = LogProvider.For<FormInstanceDataManager>();
        private FormInstanceDataManager()
        {

        }

        public FormInstanceDataManager(int tenantId, IFormInstanceDataServices formInstanceDataServices, IFolderVersionServices folderVersionServices)
        {
            this._tenantId = tenantId;
            this._formInstanceDataServices = formInstanceDataServices;
            this._folderVersionServices = folderVersionServices;
            this._loadAllSections = false;


        }

        public FormInstanceDataManager(int tenantId, int? userId, IFormInstanceDataServices formInstanceDataServices, string currentUserName, IFolderVersionServices folderVersionServices)
        {
            this._tenantId = tenantId;
            this._userId = userId;
            this._formInstanceDataServices = formInstanceDataServices;
            this._folderVersionServices = folderVersionServices;
            this._currentUserName = currentUserName;
            this._handler = new FormInstanceSectionDataCacheHandler();
            this._loadAllSections = false;
        }

        /// <summary>
        /// This Method will get data form DB if it is not present in Cache else return from cache.
        /// </summary>        
        public string GetSectionData(int formInstanceId, string sectionName, bool isRefreshCache, FormDesignVersionDetail detail, bool getFromDB, bool isMasterListAsSource)
        {
            string data = "";

            if (detail.IsMasterList && isMasterListAsSource)
            {
                data = this.GetMasterListSourcedData(formInstanceId, sectionName, isRefreshCache, detail, getFromDB);
            }
            else
            {
                data = this.GetData(formInstanceId, sectionName, isRefreshCache, detail, getFromDB);
            }
            return data;
        }

        public string GetSectionData(int formInstanceId, string sectionName, FormDesignVersionDetail detail, bool isMasterListAsSource)
        {
            string data = "";

            if (detail.IsMasterList && isMasterListAsSource)
            {
                data = this.GetMasterListSourcedData(formInstanceId, sectionName, detail);
            }
            else
            {
                data = this.GetData(formInstanceId, sectionName, detail);
            }
            return data;
        }

        /// <summary>
        /// This method will get section data from cache and update it to DB.
        /// </summary>        
        public void UpdateSectionData(int formInstanceId, string sectionName, string sectionData)
        {
            //sectionData = _handler.IsExists(formInstanceId, sectionName, _userId);
            if (!String.IsNullOrEmpty(sectionData))
            {
                JObject obj = JObject.Parse(sectionData);
                sectionData = JsonConvert.SerializeObject(obj[sectionName]);
                _formInstanceDataServices.UpdateSectionData(_tenantId, formInstanceId, sectionName, sectionData);
            }
        }

        /// <summary>
        /// This method will update section data into cache
        /// </summary>    
        public void SetCacheData(int formInstanceId, string sectionName, string sectionData)
        {
            _handler.UpdateSection(formInstanceId, sectionName, sectionData, _userId);
        }

        /// <summary>
        /// This method will save all sections of form in database
        /// </summary>    
        public ServiceResult SaveSectionsData(int formInstanceId, bool updateTargetDocument, IFolderVersionServices folderVersionServices, IFormDesignService formDesignServices, FormDesignVersionDetail detail, string secName)
        {
            this._folderVersionServices = folderVersionServices;
            this._formDesignServices = formDesignServices;
            ServiceResult result = null;

            List<JToken> formInstanceDataList = new List<JToken>();
            ICompressionBase compressionObj = CompressionFactory.GetCompressionFactory(CompressionType.JSON, null, "", "", "");
            if (!detail.IsMasterList)
            {
                List<JToken> sectionList = _handler.GetSectionListFromCache(formInstanceId, _userId);

                foreach (JToken sectionName in sectionList)
                {
                    JObject sectionData = JObject.Parse(_handler.IsExists(Convert.ToInt32(formInstanceId), sectionName.ToString(), _userId));
                    JObject obj = JObject.Parse("{'FormInstanceId':'','SectionName':'','SectionData':''}");
                    obj["FormInstanceId"] = formInstanceId;
                    obj["SectionName"] = sectionName.ToString();
                    obj["SectionData"] = compressionObj.Compress(JsonConvert.SerializeObject(sectionData[sectionName.ToString()])).ToString();
                    formInstanceDataList.Add(obj);
                }
            }
            else
            {
                JObject sectionData = JObject.Parse(_handler.IsExists(Convert.ToInt32(formInstanceId), secName.ToString(), _userId));
                JObject obj = JObject.Parse("{'FormInstanceId':'','SectionName':'','SectionData':''}");
                obj["FormInstanceId"] = formInstanceId;
                obj["SectionName"] = secName.ToString();
                obj["SectionData"] = compressionObj.Compress(JsonConvert.SerializeObject(sectionData[secName.ToString()])).ToString();
                formInstanceDataList.Add(obj);
            }
            if (formInstanceDataList.Count > 0)
            {
                result = _formInstanceDataServices.SaveFormInstanceSectionsData(_tenantId, JsonConvert.SerializeObject(formInstanceDataList), _currentUserName);
            }

            if (updateTargetDocument)
            {
                this.SaveTargetSectionsData(formInstanceId, folderVersionServices, formDesignServices);
            }

            if (detail.IsMasterList)
            {
                MasterListSectionDataCacheHandler mlHandler = new MasterListSectionDataCacheHandler();
                //foreach (SectionDesign secdesign in detail.Sections)
                //{
                //    mlHandler.RemoveSectionData(formInstanceId, secdesign.FullName.ToString());
                //}
                mlHandler.RemoveSectionData(formInstanceId, secName);
            }

            return result;
        }

        /// <summary>
        /// This method will save all section from cache of target document
        /// </summary>
        /// <param name="sourceFormInstanceId"></param>
        /// <returns></returns>
        public ServiceResult SaveTargetSectionsData(int sourceFormInstanceId, IFolderVersionServices folderVersionService, IFormDesignService formDesignService)
        {
            ServiceResult result = null;
            List<JToken> sectionList = new List<JToken>();
            List<JToken> targetFormInstanceIds = _handler.GetTargetFormInstanceIdsFromCache(sourceFormInstanceId, _userId);

            foreach (JToken targetFormInstanceId in targetFormInstanceIds)
            {
                int sourceformDesignVersionId = folderVersionService.GetSourceFormDesignVersionId(Convert.ToInt32(targetFormInstanceId));

                FormDesignVersionManager formDesignVersionMgr = new FormDesignVersionManager(_tenantId, sourceformDesignVersionId, formDesignService);
                FormDesignVersionDetail sourceDetail = formDesignVersionMgr.GetFormDesignVersion(true);

                result = this.SaveSectionsData(Convert.ToInt32(targetFormInstanceId), false, folderVersionService, formDesignService, sourceDetail, "");
            }

            return result;
        }

        private string GetData(int formInstanceId, string sectionName, FormDesignVersionDetail detail)
        {
            string data = string.Empty;
            data = _formInstanceDataServices.GetSectionData(_tenantId, formInstanceId, sectionName, detail, _currentUserName);

            string defaultJSONData = detail.GetDefaultJSONDataObject();
            VersionSyncProcessor syncProc = new VersionSyncProcessor(sectionName, defaultJSONData, data);
            data = syncProc.Run();

            return data;
        }

        private string GetData(int formInstanceId, string sectionName, bool isRefreshCache, FormDesignVersionDetail detail, bool getFromDB)
        {
            string data = "";

            if (isRefreshCache == true)
            {
                foreach (SectionDesign secdesign in detail.Sections)
                {
                    _handler.RemoveSectionData(formInstanceId, secdesign.FullName, _userId);
                }
                _handler.RevomeSectionListFromCache(formInstanceId, _userId);
                _handler.RemoveTargetFormInstanceFromCache(formInstanceId, _userId);
            }

            data = _handler.IsExists(formInstanceId, sectionName, _userId);

            if (data == string.Empty || data == null || isRefreshCache == true || getFromDB == true)
            {
                if (this._loadAllSections)
                    data = GetSectionDataByName(formInstanceId, sectionName, detail, isRefreshCache);
                else
                    data = _formInstanceDataServices.GetSectionData(_tenantId, formInstanceId, sectionName, detail, _currentUserName);

                string defaultJSONData = detail.GetDefaultJSONDataObject();
                VersionSyncProcessor syncProc = new VersionSyncProcessor(sectionName, defaultJSONData, data);
                data = syncProc.Run();

                if (getFromDB == false)
                {
                    _handler.AddSectionData(formInstanceId, sectionName, data, _userId);
                }
            }

            return data;
        }

        private string GetSectionDataByName(int formInstanceId, string sectionName, FormDesignVersionDetail detail, bool isRefreshCache)
        {
            string sectionData = string.Empty;
            IDataCacheHandler cacheHandler = DataCacheHandlerFactory.GetHandler();
            string formData = cacheHandler.Get(_tenantId, formInstanceId, isRefreshCache, detail, _folderVersionServices, _userId);
            // Add DefaultJsonData to DB for newally created document
            if (cacheHandler.IsNewFormInstance && !string.IsNullOrEmpty(formData))
            {
                string defaultJSONData = detail.JSONData;
                _formInstanceDataServices.SaveDefaultJSONData(_tenantId, formInstanceId, defaultJSONData, _currentUserName);
                formData = defaultJSONData;
            }
            JObject objFormData = JObject.Parse(formData);
            JObject objSectionData = JObject.Parse("{'" + sectionName + "':[]}");
            objSectionData[sectionName] = objFormData[sectionName];
            return sectionData = JsonConvert.SerializeObject(objSectionData);
        }

        private string GetMasterListSourcedData(int formInstanceId, string sectionName, bool isRefreshCache, FormDesignVersionDetail detail, bool getFromDB)
        {
            MasterListSectionDataCacheHandler mlHandler = new MasterListSectionDataCacheHandler();
            string data = "";

            if (isRefreshCache == true)
            {
                foreach (SectionDesign secdesign in detail.Sections)
                {
                    mlHandler.RemoveSectionData(formInstanceId, secdesign.FullName);
                }
            }

            data = mlHandler.IsExists(formInstanceId, sectionName);

            if (data == string.Empty || data == null || isRefreshCache == true || getFromDB == true)
            {
                data = _formInstanceDataServices.GetSectionData(_tenantId, formInstanceId, sectionName, detail, _currentUserName);

                string defaultJSONData = detail.GetDefaultJSONDataObject();
                VersionSyncProcessor syncProc = new VersionSyncProcessor(sectionName, defaultJSONData, data);
                data = syncProc.Run();

                if (getFromDB == false)
                {
                    mlHandler.AddSectionData(formInstanceId, sectionName, data);
                }
            }

            return data;
        }

        private string GetMasterListSourcedData(int formInstanceId, string sectionName, FormDesignVersionDetail detail)
        {
            string data = "";
            data = _formInstanceDataServices.GetSectionData(_tenantId, formInstanceId, sectionName, detail, _currentUserName);

            string defaultJSONData = detail.GetDefaultJSONDataObject();
            VersionSyncProcessor syncProc = new VersionSyncProcessor(sectionName, defaultJSONData, data);
            data = syncProc.Run();

            return data;
        }


        public void RemoveSectionListFromCache(int formInstanceId, int userId)
        {
            _handler.RevomeSectionListFromCache(formInstanceId, userId);
        }

        public void RemoveSectionsData(int formInstanceId, string sectionName, int userId)
        {
            _handler.RemoveSectionData(formInstanceId, sectionName, userId);
        }

        public List<JToken> GetSectionsFromCache(int formInstanceId, int userId)
        {
            return _handler.GetSectionListFromCache(formInstanceId, userId);
        }

        public bool SaveFormInstanceDataIntoReportingCenterDB(int tenantId, int? CurrentUserId, string CurrentUserName, int formInstanceId, int folderId)
        {
            try
            {
                this._folderVersionServices.UpdateReportingCenterDatabase(formInstanceId, null, false);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ReportingCenter database data update transaction fail Before reaching to Genrating SQL Statement. FormInstanceId='" + formInstanceId + "'", ex);
            }
            return true;
        }

    }
}