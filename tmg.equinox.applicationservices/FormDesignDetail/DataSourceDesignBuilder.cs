using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;
using tmg.equinox.repository.extensions;
using tmg.equinox.applicationservices.viewmodels.UIElement;

namespace tmg.equinox.applicationservices.FormDesignDetail
{
    internal class DataSourceDesignBuilder
    {
        int formDesignVersionId;
        int formDesignId;
        int tenantId;
        List<UIElement> formElementList;
        IUnitOfWorkAsync _unitOfWork;
        MasterLists msLists;
        Dictionary<int, List<UIElement>> dataSourceDesignFormElementList = new Dictionary<int, List<UIElement>>();
        internal DataSourceDesignBuilder(int tenantId, int formDesignId, int formDesignVersionId, List<UIElement> formElementList, IUnitOfWorkAsync unitOfWork, MasterLists msLists)
        {
            this.formDesignId = formDesignId;
            this.formDesignVersionId = formDesignVersionId;
            this.tenantId = tenantId;
            this.formElementList = formElementList;
            this._unitOfWork = unitOfWork;
            this.msLists = msLists;
        }

        internal List<DataSourceDesign> GetDataSources()
        {
            List<DataSourceDesign> designs = new List<DataSourceDesign>();
            var dataSourceList = (from ds in this._unitOfWork.RepositoryAsync<DataSourceMapping>()
                                                        .Query()
                                                        .Include(b => b.DataSource)
                                                        .Include(c => c.UIElement)
                                                        .Include(d => d.UIElement1)
                                                        .Include(e => e.UIElement1.DataSourceElementDisplayMode)
                                                        .Include(e => e.DataSourceMode)
                                                        .Get()
                                  where ds.FormDesignVersionID == formDesignVersionId
                                  select new
                                  {
                                      //FormDesignID = ds.UIElement1.FormID,
                                      FormDesignID = ds.DataSource.FormDesignID,

                                      SourceParentUIElementID = ds.UIElement1.ParentUIElementID.HasValue ? ds.UIElement1.ParentUIElementID.Value : 0,
                                      TargetParentUIElementID = ds.UIElement.ParentUIElementID.HasValue ? ds.UIElement.ParentUIElementID.Value : 0,
                                      DisplayModeUIElement = ds.UIElement.DataSourceElementDisplayMode,
                                      DisplayMode = ds.DataSourceElementDisplayMode,
                                      DataSourceModeType = ds.DataSourceMode.DataSourceModeType,
                                      DataSourceName = ds.DataSource.DataSourceName,
                                      IsPrimary = ds.IsPrimary,
                                      SourceUIElementID = ds.MappedUIElementID,
                                      TargetUIElementID = ds.UIElementID,
                                      SourceUIElementName = ds.UIElement1.GeneratedName,
                                      TargetUIElementName = ds.UIElement.GeneratedName,
                                      Sequence = ds.UIElement.Sequence,
                                      IsKey = ds.IsKey,
                                      Filter = ds.DataSourceFilter,
                                      Operator = ds.DataSourceOperatorID,
                                      CopyMode = ds.DataCopyModeID,
                                      IncludeChild = ds.IncludeChild,
                                  }).ToList();
            if (dataSourceList != null && dataSourceList.Count() > 0)
            {
                Dictionary<string, string> generatedNameDS = new Dictionary<string, string>();
                
                var designGroups = from ds in dataSourceList
                                   group ds by new { ds.TargetParentUIElementID, ds.DisplayMode.DisplayMode } into grouping
                                   select grouping;
                foreach (var dsGroup in designGroups)
                {
                    var srcGroups = dsGroup.Key.DisplayMode == "Dropdown" ?
                                    (from src in dsGroup
                                     group src by new { src.DataSourceName, src.TargetUIElementID } into srcgroup
                                     select srcgroup) :
                                    (from src in dsGroup
                                     group src by new { src.DataSourceName, TargetUIElementID = 1 } into srcgroup
                                     select srcgroup);
                    foreach (var srcGroup in srcGroups)
                    {
                        var sr = from src in srcGroup select src;
                        if (sr != null && sr.Count() > 0)
                        {
                            var items = sr.ToList();
                            DataSourceDesign design = new DataSourceDesign();
                            design.DisplayMode = items[0].DisplayMode != null ? items[0].DisplayMode.DisplayMode : "";
                            design.FormDesignID = items[0].FormDesignID;
                            //design.FormDesignID = items[0].FormDesignID == formDesignId ? GetFormDesignID(items[0].DataSourceName) :
                            //                                                    items[0].FormDesignID;
                            design.DataSourceModeType = items[0].DataSourceModeType;
                            design.MappingType = design.MappingType;
                            if (generatedNameDS.ContainsKey(items[0].DataSourceName))
                                design.DataSourceName = generatedNameDS[items[0].DataSourceName];
                            else
                            {
                                design.DataSourceName = GetGeneratedName(items[0].DataSourceName);
                                generatedNameDS.Add(items[0].DataSourceName, design.DataSourceName);
                            }
                            var sourceParentElemntID = items[0].FormDesignID == formDesignId ? GetSourceParentUIElementID(items[0].DataSourceName, items[0].SourceParentUIElementID, design.FormDesignID) :
                                                                    items[0].SourceParentUIElementID;
                            design.SourceParent = GetGeneratedNameForDataSource(design.FormDesignID, sourceParentElemntID);
                            design.TargetParent = GetGeneratedNameForDataSource(items[0].TargetParentUIElementID, formElementList);
                            var mappings = (from item in items.OrderBy(c => c.Sequence)
                                            select new DataSourceElementMapping
                                            {
                                                SourceElement = item.SourceUIElementName,
                                                TargetElement = item.TargetUIElementName,
                                                IsKey = item.IsKey,
                                                CopyModeID = item.CopyMode.HasValue == true ? item.CopyMode.Value : 0,
                                                OperatorID = item.Operator.HasValue == true ? item.Operator.Value : 0,
                                                Filter = item.Filter,
                                                IncludeChild = item.IncludeChild
                                            });
                            if (mappings != null && mappings.Count() > 0)
                            {
                                design.Mappings = mappings.ToList();
                            }
                            else
                            {
                                design.Mappings = new List<DataSourceElementMapping>();
                            }
                            designs.Add(design);
                        }
                    }
                }
            }
            if (designs != null && designs.Count > 0)
            {
                designs = OrderDesigns(designs);
            }

            return designs;
        }

        private List<DataSourceDesign> OrderDesigns(List<DataSourceDesign> designs)
        {
            List<DataSourceDesign> orderedDesigns = new List<DataSourceDesign>();

            var sectionDataSources = from des in designs
                                     where des.DisplayMode == "Section"
                                     select des;
            //add Section data sources first
            if (sectionDataSources != null && sectionDataSources.Count() > 0)
            {
                orderedDesigns.AddRange(sectionDataSources.ToList());
            }

            //primary data sources
            var primaryDataSources = from des in designs
                                     where des.DisplayMode == "Primary"
                                     select des;
            if (primaryDataSources != null && primaryDataSources.Count() > 0)
            {
                List<DataSourceDesign> primaries = OrderByDesignSource(primaryDataSources.ToList());
                orderedDesigns.AddRange(primaries);
            }

            //child data sources
            var childDataSources = (from des in designs
                                    where des.DisplayMode == "Child" || des.DisplayMode == "In Line"
                                    select des).ToList();
            if (childDataSources != null && childDataSources.Count() > 0)
            {
                orderedDesigns.AddRange(childDataSources.ToList());
            }

            //dropdowns
            var dropdowns = from des in designs
                            where des.DisplayMode == "Dropdown"
                            select des;
            if (dropdowns != null && dropdowns.Count() > 0)
            {
                orderedDesigns.AddRange(dropdowns.ToList());
            }
            return orderedDesigns;
        }

        private List<DataSourceDesign> OrderByDesignSource(List<DataSourceDesign> designs)
        {
            //get list where target is not source for another source
            List<DataSourceDesign> orderedDesigns = new List<DataSourceDesign>();
            var targetParents = from des in designs where des.FormDesignID == formDesignId select des.TargetParent;
            int childCount = 1;
            var desParents = from de in designs where !targetParents.Contains(de.SourceParent) select de;
            var desChildren = from de in designs where targetParents.Contains(de.SourceParent) select de;
            while (childCount > 0)
            {
                if (desParents != null && desParents.Count() > 0)
                {
                    orderedDesigns.AddRange(desParents.ToList());
                }
                if (desChildren != null && desChildren.Count() > 0)
                {
                    List<DataSourceDesign> children = desChildren.ToList();
                    targetParents = from des in children select des.TargetParent;
                    desParents = (from des in children where !targetParents.Contains(des.SourceParent) select des).ToList();
                    desChildren = from des in children where targetParents.Contains(des.SourceParent) select des;
                }
                else
                {
                    childCount = 0;
                }
            }
            return orderedDesigns;
        }

        private string GetGeneratedNameForDataSource(int formDesignID, int elementID)
        {
            List<UIElement> formElementList = GetDataSourceElementListByDesignID(formDesignID);
            return GetGeneratedNameForDataSource(elementID, formElementList);
        }

        private string GetGeneratedNameForDataSource(int elementID, List<UIElement> formElementList)
        {
            formElementList = (from el in formElementList where el.IsContainer == true select el).ToList();
            string generatedName = "";
            UIElement element = (from elem in formElementList
                                 where elem.UIElementID == elementID
                                 select elem).FirstOrDefault();
            int currentElementID = elementID;
            int parentUIElementID = element.ParentUIElementID.HasValue ? element.ParentUIElementID.Value : 0;
            generatedName = element.GeneratedName;
            while (parentUIElementID > 0)
            {
                element = (from elem in formElementList
                           where elem.UIElementID == parentUIElementID
                           select elem).FirstOrDefault();
                parentUIElementID = element.ParentUIElementID.HasValue ? element.ParentUIElementID.Value : 0;
                if (parentUIElementID > 0)
                {
                    generatedName = element.GeneratedName + "." + generatedName;
                }
            }
            return generatedName;
        }

        private string GetGeneratedName(string label)
        {
            string generatedName = "";
            if (!String.IsNullOrEmpty(label))
            {
                Regex regex = new Regex("[^a-zA-Z0-9]");
                generatedName = regex.Replace(label, String.Empty);
                if (generatedName.Length > 70)
                {
                    generatedName = generatedName.Substring(0, 70);
                }

                Regex checkDigits = new Regex("^[0-9]");

                //if Label starts with numeric characters, this will append a character at the beginning.
                if (checkDigits.IsMatch(label, 0))
                {
                    generatedName = "a" + generatedName;
                }
            }
            return generatedName;
        }

        private int GetFormDesignID(string dataSourceName)
        {
            var formDesignID = this._unitOfWork.RepositoryAsync<DataSource>()
                                    .Query()
                                    .Filter(fil => fil.DataSourceName == dataSourceName)
                                    .Get()
                                    .Select(sel => sel.FormDesignID)
                                    .FirstOrDefault();
            return formDesignID;
        }

        private int GetSourceParentUIElementID(string dataSourceName, int defaultParentElementID, int formDesignID)
        {
            var formDesign = this._unitOfWork.RepositoryAsync<RepeaterUIElement>()
                                                     .Query()
                                                     .Filter(fil =>  fil.DataSource.FormDesignID == formDesignID && fil.DataSource.DataSourceName == dataSourceName)
                                                     .Get().FirstOrDefault();

            if (formDesign == null)
            {
                return defaultParentElementID;
            }
            return formDesign.UIElementID;
        }

        private List<UIElement> GetDataSourceElementListByDesignID(int formDesignId)
        {
            List<UIElement> uiElementList = new List<UIElement>();
            if (dataSourceDesignFormElementList.ContainsKey(formDesignId))
            {
                uiElementList = dataSourceDesignFormElementList[formDesignId];
            }
            else
            {
                uiElementList = (from u in this._unitOfWork.Repository<UIElement>()
                                                        .Query()
                                                        .Get()
                                 join fd in this._unitOfWork.Repository<FormDesignVersionUIElementMap>()
                                                         .Query()
                                                         .Get()
                                 on u.UIElementID equals fd.UIElementID
                                 where fd.FormDesignVersion.FormDesignID == formDesignId
                                 && u.IsContainer == true
                                 select u).ToList();
                if (uiElementList != null)
                    dataSourceDesignFormElementList.Add(formDesignId, uiElementList);
            }
            return uiElementList;
        }
    }
}
