using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.web.CustomRule;
using tmg.equinox.web.Framework.Caching;

namespace tmg.equinox.web.DataSource.SyncManager
{
    public class DataSourceSynchManager
    {
        JObject _oldFormData { get; set; }
        JObject _formData { get; set; }
        FormDesignVersionDetail _formDesign { get; set; }
        List<SectionResult> updatedSections { get; set; }

       // readonly string[] syncExcludedSections = { "BenefitReview" };

        public DataSourceSynchManager(string oldFormData, string formDesignData)
        {
            this._oldFormData = JObject.Parse(oldFormData);
            this._formDesign = JsonConvert.DeserializeObject<FormDesignVersionDetail>(formDesignData);
            _formData = _formData == null ? (JObject)_oldFormData.DeepClone() : _formData;
            updatedSections = new List<SectionResult>();
        }

        public DataSourceSynchManager(JObject oldFormData, string newFormData, FormDesignVersionDetail formDesign)
        {
            this._oldFormData = oldFormData;
            this._formDesign = formDesign;
            _formData = string.IsNullOrEmpty(newFormData) ? (JObject)_oldFormData.DeepClone() : JObject.Parse(newFormData);
            updatedSections = new List<SectionResult>();
        }


        public string SyncFormDataSources()
        {
            foreach (var item in _formData)
            {
                SyncSectionDataSources(item.Key, true);
            }
            return JsonConvert.SerializeObject(_formData);
        }

        public string SyncSectionDataSources(string sectionName, string sectionData)
        {
            updatedSections = new List<SectionResult>();
            JObject oldSectionData = (JObject)_formData[sectionName].DeepClone();
            _formData[sectionName] = JObject.Parse(sectionData);
            SyncSectionDataSources(sectionName, false);
            _formData[sectionName] = oldSectionData;
            return JsonConvert.SerializeObject(_formData);
        }

        public List<SectionResult> GetUpdatedSections()
        {
            return updatedSections;
        }

        #region Private Methods
        private void SyncSectionDataSources(string sectionName, bool syncSameSectionDataSource)
        {
            List<DataSourceDesign> sectionDataSources = GetDataSourcesBySectionName(sectionName);
            var dataSourceList = sectionDataSources.Where(d => d.DataSourceModeType == "Auto" && d.DisplayMode != "Dropdown").ToList();
            if (syncSameSectionDataSource == false)
            {
                dataSourceList = dataSourceList.Where(d => !d.TargetParent.StartsWith(sectionName + ".")).ToList();
            }
            foreach (DataSourceDesign dataSource in dataSourceList)
            {
                //if (!syncExcludedSections.Contains(dataSource.TargetParent.Split('.')[0]))
                //{
                    ExecuteSynchronization(dataSource);
                //}
            }
        }

        private void ExecuteSynchronization(DataSourceDesign dataSource)
        {

            JArray sourceData = GetRepeaterData(dataSource.SourceParent, _formData);
            JArray oldSourceData = GetRepeaterData(dataSource.SourceParent, _oldFormData);
            JArray targetData = GetRepeaterData(dataSource.TargetParent, _formData);
            sourceData = GetFilteredDataSourceData(sourceData, dataSource);
            var difference = GetDifference(oldSourceData, sourceData, dataSource);
            if (difference.Any())
            {
                RepeaterDesign repeaterDesign = getTargetRepeaterDesign(dataSource);

                switch (dataSource.DisplayMode)
                {
                    case "Primary":
                        {
                            PrimaryDSSynchroniser primarySync = new PrimaryDSSynchroniser(sourceData, targetData, dataSource, repeaterDesign, difference);
                            primarySync.syncMapDataSource();
                            string newSectionName = dataSource.TargetParent.Split('.')[0];
                            var targetParentDataSourcesAsSource = _formDesign.DataSources.Where(d => d.FormDesignID == _formDesign.FormDesignId && d.SourceParent == dataSource.TargetParent);
                            if (targetParentDataSourcesAsSource.Any())
                            {
                                SyncSectionDataSources(newSectionName, true);
                            }
                            AddToUpdatedSections(newSectionName, primarySync.isTargetDataUpdated);
                            break;
                        }
                    case "Child":
                    case "In Line":
                        {
                            ChildInlineDSSynchroniser inlineSync = new ChildInlineDSSynchroniser(sourceData, targetData, dataSource, repeaterDesign, difference);
                            inlineSync.syncMapDataSource();
                            string newSectionName = dataSource.TargetParent.Split('.')[0];
                            AddToUpdatedSections(newSectionName, inlineSync.isTargetDataUpdated);
                            break;
                        }
                    case "Dropdown":
                        /// Not implemented yet.
                        break;
                }
            }
        }

        private List<DataSourceDesign> GetDataSourcesBySectionName(string sectionName)
        {
            List<DataSourceDesign> sectionDataSources = _formDesign.DataSources.Where(f => f.FormDesignID == _formDesign.FormDesignId && f.SourceParent.StartsWith(sectionName + ".")).ToList();
            return sectionDataSources;
        }

        private JArray GetRepeaterData(string fullName, JObject dataContainer)
        {
            string[] fullNameParts = fullName.Split('.');
            JToken data = null;
            foreach (string partName in fullNameParts)
            {
                data = data == null ? dataContainer[partName] : data[partName];
            }

            return data == null ? null : (JArray)data;
        }

        private Dictionary<string, List<JToken>> GetDifference(JArray oldData, JArray newData, DataSourceDesign dataSource)
        {
            Dictionary<string, List<JToken>> differece = new Dictionary<string, List<JToken>>();
            Dictionary<string, string> keysToCompare = dataSource.Mappings.Where(m => m.IsKey).ToDictionary(s => s.SourceElement, s => s.SourceElement);
            var jTokenComparer = new JTokenEqualityComparer(keysToCompare);
            List<JToken> addedRows = newData.Except<JToken>(oldData, jTokenComparer).ToList();
            List<JToken> deletedRows = oldData.Except<JToken>(newData, jTokenComparer).ToList();
            List<JToken> commonRows = newData.Intersect<JToken>(oldData, jTokenComparer).ToList();
            differece.Add("Add", addedRows);
            differece.Add("Delete", deletedRows);
            differece.Add("Update", commonRows);
            return differece;
        }

        private RepeaterDesign getTargetRepeaterDesign(DataSourceDesign dataSource)
        {
            string[] fullNameArray = dataSource.TargetParent.Split('.');
            SectionDesign targetParentSection = _formDesign.Sections.Where(s => s.GeneratedName == fullNameArray[0]).FirstOrDefault();
            ElementDesign targetRepeaterDesignElement = null;
            if (fullNameArray.Length > 1)
            {
                for (int i = 1; i < fullNameArray.Length; i++)
                {
                    if (targetRepeaterDesignElement == null)
                    {
                        targetRepeaterDesignElement = targetParentSection.Elements.Where(e => e.GeneratedName == fullNameArray[i]).FirstOrDefault();
                    }
                    else
                    {
                        targetRepeaterDesignElement = targetRepeaterDesignElement.Section.Elements.Where(e => e.GeneratedName == fullNameArray[i]).FirstOrDefault();
                    }
                }
            }
            return targetRepeaterDesignElement == null ? null : targetRepeaterDesignElement.Repeater;
        }

        private void AddToUpdatedSections(string sectionName, bool isSectionUpdated)
        {
            if (isSectionUpdated)
            {
                var updatedSection = updatedSections.Where(s => s.SectionName == sectionName).FirstOrDefault();
                if (updatedSection != null)
                {
                    updatedSection.SectionData = JsonConvert.SerializeObject(_formData[sectionName]);
                }
                else
                {
                    SectionResult result = new SectionResult(sectionName, JsonConvert.SerializeObject(_formData[sectionName]));
                    updatedSections.Add(result);
                }
            }
        }

        private JArray GetFilteredDataSourceData(JArray data, DataSourceDesign dataSource)
        {
            try
            {
                List<DataSourceElementMapping> filteredMaps = dataSource.Mappings.Where(m => m.Filter != null && m.OperatorID > 0).ToList();
                if (filteredMaps.Any())
                {
                    foreach (var mapping in filteredMaps)
                    {
                        double elementValue = 0;
                        double filterValue = 0;
                        switch (mapping.OperatorID)
                        {
                            case 1: //equals                            
                                data = JArray.FromObject(data.Where(r => r[mapping.SourceElement] != null && Convert.ToString(r[mapping.SourceElement]).ToUpper() == Convert.ToString(mapping.Filter).ToUpper()).ToList());
                                break;
                            case 2: //greater than                                                       
                                data = JArray.FromObject(data.Where(r => r[mapping.SourceElement] != null && double.TryParse(r[mapping.SourceElement].ToString(), out elementValue) && double.TryParse(mapping.Filter, out filterValue) && elementValue > filterValue).ToList());
                                break;
                            case 3: //less than
                                data = JArray.FromObject(data.Where(r => r[mapping.SourceElement] != null && double.TryParse(r[mapping.SourceElement].ToString(), out elementValue) && double.TryParse(mapping.Filter, out filterValue) && elementValue < filterValue).ToList());
                                break;
                            case 4: //greater than or equals                            
                                data = JArray.FromObject(data.Where(r => r[mapping.SourceElement] != null && double.TryParse(r[mapping.SourceElement].ToString(), out elementValue) && double.TryParse(mapping.Filter, out filterValue) && elementValue >= filterValue).ToList());
                                break;
                            case 5: //less than or equals                            
                                data = JArray.FromObject(data.Where(r => r[mapping.SourceElement] != null && double.TryParse(r[mapping.SourceElement].ToString(), out elementValue) && double.TryParse(mapping.Filter, out filterValue) && elementValue <= filterValue).ToList());
                                break;
                            case 6: //contains
                                data = JArray.FromObject(data.Where(r => r[mapping.SourceElement] != null && Convert.ToString(r[mapping.SourceElement]).ToUpper().Contains(Convert.ToString(mapping.Filter).ToUpper())).ToList());
                                break;
                            case 7: //is null
                                data = JArray.FromObject(data.Where(r => r[mapping.SourceElement] != null && Convert.ToString(r[mapping.SourceElement]) == null).ToList());
                                break;
                            case 8: //not equals
                                data = JArray.FromObject(data.Where(r => r[mapping.SourceElement] != null && Convert.ToString(r[mapping.SourceElement]).ToUpper() != Convert.ToString(mapping.Filter).ToUpper()).ToList());
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return data;
        }
        #endregion
    }
}