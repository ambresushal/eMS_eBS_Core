using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Dynamic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.web.CustomRule;
using Newtonsoft.Json.Linq;
using tmg.equinox.web.FormInstanceManager;
using tmg.equinox.web.FormDesignManager;

namespace tmg.equinox.web.DataSource
{
    public class DataSourceMapper
    {
        private int tenantID;
        private int formInstanceID;
        private int folderVersionID;
        private int formDesignID;
        private int formDesignVersionID;
        private bool isfolderReleased;
        private string formInstanceData;
        private IFolderVersionServices folderVersionServices;
        private List<DataSourceDesign> designs;
        private FormDesignVersionDetail formDesign;
        private Dictionary<int, SourceInstanceData> sourceInstances;
        private FormInstanceDataManager _formDataInstanceManager;
        private string _sectionName;
        private IFormDesignService _formDesignServices;

        public DataSourceMapper(int tenantID, int formInstanceID, int folderVersionID, int formDesignID, int formDesignVersionID, bool isfolderReleased, IFolderVersionServices folderVersionServices, string formInstanceData, FormDesignVersionDetail formDesign, FormInstanceDataManager formDataInstanceManager, string sectionName, IFormDesignService formDesignServices)
        {
            this.tenantID = tenantID;
            this.formInstanceID = formInstanceID;
            this.folderVersionID = folderVersionID;
            this.formDesignID = formDesignID;
            this.formDesignVersionID = formDesignVersionID;
            this.isfolderReleased = isfolderReleased;
            this.formInstanceData = formInstanceData;
            this.folderVersionServices = folderVersionServices;
            this.formDesign = formDesign;
            this._formDataInstanceManager = formDataInstanceManager;
            designs = new List<DataSourceDesign>();
            sourceInstances = new Dictionary<int,SourceInstanceData>();
            this._sectionName = sectionName;
            this._formDesignServices = formDesignServices;
        }

        public void AddDataSource(DataSourceDesign design)
        {
            this.designs.Add(design);
        }

        public void AddDataSourceRange(List<DataSourceDesign> designs)
        {
            this.designs.AddRange(designs);
        }

        public string MapDataSources()
        {
            string json = "";
            if (designs.Count > 0)
            {
                var converter = new ExpandoObjectConverter();
                dynamic jsonObject = JsonConvert.DeserializeObject<ExpandoObject>(this.formInstanceData, converter);
                ExpandoObject source = null;
                List<DataSourceDesign> sourceDesign = this.designs;
                if (!string.IsNullOrEmpty(_sectionName))
                {
                    sourceDesign = this.designs.Where(a => a.TargetParent.Split('.')[0].ToString() == this._sectionName).ToList();
                }

                foreach (DataSourceDesign design in sourceDesign)
                {
                    if (this.formDesignID == design.FormDesignID)
                    {
                        string sourceData = _formDataInstanceManager.GetSectionData(this.formInstanceID, design.SourceParent.Split('.')[0], false, formDesign, false, false);
                        source = JsonConvert.DeserializeObject<ExpandoObject>(sourceData, converter);
                    }
                    else
                    {
                        source = GetDataSource(design);
                    }
                    switch (design.DisplayMode)
                    {
                        case "Section":
                            if (!this.isfolderReleased)
                            {
                                SectionDataSourceMapper sectionMapper = new SectionDataSourceMapper(design, source, jsonObject);
                                sectionMapper.MergeDataSource();
                            }
                            break;
                        case "Primary":
                        case "In Line":
                        case "Child":
                            if (!this.isfolderReleased)
                            {
                                RepeaterDataSourceMapper repeaterMapper = new RepeaterDataSourceMapper(design, formDesign, source, jsonObject);
                                repeaterMapper.MergeDataSource();
                            }
                            break;
                        case "Dropdown":
                            DropDownDataSourceMapper dropdownMapper = new DropDownDataSourceMapper(design, source, formDesign);
                            dropdownMapper.MergeDataSource();
                            break;
                    }
                }
                json = JsonConvert.SerializeObject(jsonObject);
            }
            else
            {
                json = this.formInstanceData;
            }
            return json;
        }

        private ExpandoObject GetDataSource(DataSourceDesign design)
        {
            ExpandoObject sourceJsonObject = null;
            int formDesignID = design.FormDesignID;
            string sectionParent = design.SourceParent.Split('.')[0];
            int sourceFormInstanceID;
            int sourceformDesignVersionId;
            if (!sourceInstances.ContainsKey(formDesignID))
            {
                sourceFormInstanceID = this.folderVersionServices.GetSourceFormInstanceID(this.formInstanceID, this.formDesignVersionID, this.folderVersionID, design.FormDesignID);
            }
            else
            {
                sourceFormInstanceID = sourceInstances[formDesignID].FormInstanceID;
            }

            string sourceFormInstanceData = "";

            if (sourceFormInstanceID != 0)
            {
                if (!sourceInstances.ContainsKey(formDesignID))
                {
                    sourceformDesignVersionId = this.folderVersionServices.GetSourceFormDesignVersionId(sourceFormInstanceID);
                }
                else
                {
                    sourceformDesignVersionId = sourceInstances[formDesignID].FormDesignVersionID;
                }


                FormDesignVersionManager formDesignVersionMgr = new FormDesignVersionManager(this.tenantID, sourceformDesignVersionId, _formDesignServices);

                FormDesignVersionDetail sourceDetail = formDesignVersionMgr.GetFormDesignVersion(true);

                bool isMasterListAsSource = false;
                if (sourceDetail.IsMasterList)
                {
                    isMasterListAsSource = true;
                }
                var converter = new ExpandoObjectConverter();

                if (!sourceInstances.ContainsKey(formDesignID))
                {
                    sourceInstances.Add(formDesignID, new SourceInstanceData{ FormInstanceID = sourceFormInstanceID, FormDesignVersionID = sourceformDesignVersionId, SectionDataDictionary = new Dictionary<string, ExpandoObject>()  });
                    sourceFormInstanceData = _formDataInstanceManager.GetSectionData(sourceFormInstanceID, sectionParent, false, sourceDetail, false, isMasterListAsSource);
                    sourceJsonObject = JsonConvert.DeserializeObject<ExpandoObject>(sourceFormInstanceData, converter);
                    sourceInstances[formDesignID].SectionDataDictionary.Add(sectionParent, sourceJsonObject);
                }
                else
                {
                    if (!sourceInstances[formDesignID].SectionDataDictionary.ContainsKey(sectionParent))
                    {
                        sourceFormInstanceData = _formDataInstanceManager.GetSectionData(sourceFormInstanceID, sectionParent, false, sourceDetail, false, isMasterListAsSource);
                        sourceJsonObject = JsonConvert.DeserializeObject<ExpandoObject>(sourceFormInstanceData, converter);
                        sourceInstances[formDesignID].SectionDataDictionary.Add(sectionParent, sourceJsonObject);
                    }
                    else
                    {
                        sourceJsonObject = sourceInstances[formDesignID].SectionDataDictionary[sectionParent];
                    }
                }

            }
            return sourceJsonObject;
        }
    }

    internal class DataSourceMappingConstants
    {
        //public const int MASTERLISTFORMDESIGNID = 1;
    }

    internal class SourceInstanceData
    {
        internal int FormInstanceID { get; set; }
        internal int FormDesignVersionID { get; set; }
        internal Dictionary<string, ExpandoObject> SectionDataDictionary { get; set; }
    }
}