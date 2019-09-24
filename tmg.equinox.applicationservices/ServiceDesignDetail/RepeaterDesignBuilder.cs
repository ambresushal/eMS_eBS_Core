using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.ServiceDesign.ServiceDesignVersion;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.applicationservices.ServiceDesignDetail
{
    internal class RepeaterDesignBuilder
    {
        ServiceDefinition repeater;
        IEnumerable<ServiceDefinition> serviceDefinitionList;
        RepeaterUIElement repeaterUIElement;
        IEnumerable<UIElement> formElementList;
        List<DataSourceDesign> dataSourceList;
        IUnitOfWorkAsync _unitOfWork;
        tmg.equinox.applicationservices.FormDesignDetail.MasterLists msList;

        internal RepeaterDesignBuilder(ServiceDefinition serviceElement, IEnumerable<ServiceDefinition> serviceDefinitionList, RepeaterUIElement repeaterElement, IEnumerable<UIElement> formElementList, List<DataSourceDesign> dataSourceList, tmg.equinox.applicationservices.FormDesignDetail.MasterLists msList, IUnitOfWorkAsync unitOfWork)
        {
            this.repeater = serviceElement;
            this.serviceDefinitionList = serviceDefinitionList;
            this.repeaterUIElement = repeaterElement;
            this.formElementList = formElementList;
            this.dataSourceList = dataSourceList;
            this._unitOfWork = unitOfWork;
            this.msList = msList;
        }

        internal RepeaterDesign BuildRepeater(string fullParentName)
        {
            RepeaterDesign design = null;
            if (repeater != null)
            {
                design = new RepeaterDesign();
                design.Elements = new List<ElementDesign>();
                design.ID = repeater.ServiceDefinitionID;
                design.UIElementID = repeater.UIElementID;
                design.DisplayName = repeater.DisplayName;
                design.GeneratedName = GetGeneratedName();
                design.UIElementFullName = ServiceDesignBuilder.GetUIElementFullPath(repeaterUIElement, formElementList.ToList());

                if (this.dataSourceList != null && this.dataSourceList.Count > 0)
                {
                    string compareName = fullParentName;
                    var pri = (from des in this.dataSourceList
                               where des.TargetParent == compareName &&
                                   des.DisplayMode == "Primary"
                               select des).FirstOrDefault();
                    if (pri != null)
                    {
                        design.PrimaryDataSource = pri;
                    }
                    var children = from des in this.dataSourceList
                                   where des.TargetParent == compareName &&
                                        (//des.DisplayMode == "In Line" || 
                                        des.DisplayMode == "Child")
                                   select des;
                    if (children != null && children.Count() > 0)
                    {
                        design.ChildDataSources = children.ToList();
                    }

                    var inline = from des in this.dataSourceList
                                 where des.TargetParent == compareName &&
                                      (des.DisplayMode == "In Line")
                                 //|| des.DisplayMode == "Child")
                                 select des;
                    if (inline != null && inline.Count() > 0)
                    {
                        design.InlineDataSources = inline.ToList();
                    }
                }
                GetChildElementDesigns(design, "child", fullParentName + "." + design.GeneratedName);
            }
            return design;
        }

        private void GetChildElementDesigns(RepeaterDesign design, string isParent, string fullParentName)
        {
            var elements = from t in repeaterUIElement.UIElement1
                           join u in formElementList
                           on t.UIElementID equals u.UIElementID
                           orderby t.Sequence
                           select t;
            var serviceElements = from t in repeater.ServiceDefinitions
                                  join u in serviceDefinitionList
                                  on t.ServiceDefinitionID equals u.ServiceDefinitionID
                                  orderby t.Sequence
                                  select t;
            if (elements != null && elements.Count() > 0 && serviceElements != null && serviceElements.Count() > 0)
            {
                foreach (var elem in serviceElements)
                {
                    UIElement element = elements.Where(c => c.UIElementID == elem.UIElementID).FirstOrDefault();
                    UIElementDesignBuilder builder = new UIElementDesignBuilder(elem, serviceDefinitionList, element, formElementList, dataSourceList, msList, _unitOfWork);
                    ElementDesign elementDesign = builder.BuildElement(fullParentName);
                    elementDesign.IsKey = elem.IsKey;
                    elementDesign.IsPrimary = true;
                    elementDesign.DataSourceMode = ElementDataSourceMode.Primary;
                    if (design.ChildDataSources != null && design.ChildDataSources.Count() > 0)
                    {
                        foreach (var dataSource in design.ChildDataSources)
                        {
                            var match = (from el in dataSource.Mappings
                                         where
                                             el.TargetElement == elementDesign.GeneratedName
                                         select el.TargetElement).FirstOrDefault();
                            if (!string.IsNullOrEmpty(match))
                            {
                                elementDesign.IsPrimary = false;
                                elementDesign.DataSourceMode = ElementDataSourceMode.Child;
                                break;
                            }
                        }
                    }

                    if (design.InlineDataSources != null && design.InlineDataSources.Count() > 0)
                    {
                        foreach (var dataSource in design.InlineDataSources)
                        {
                            var match = (from el in dataSource.Mappings
                                         where
                                             el.TargetElement == elementDesign.GeneratedName
                                         select el.TargetElement).FirstOrDefault();
                            if (!string.IsNullOrEmpty(match))
                            {
                                elementDesign.IsPrimary = false;
                                elementDesign.DataSourceMode = ElementDataSourceMode.Inline;
                                break;
                            }
                        }
                    }

                    design.Elements.Add(elementDesign);
                }
            }
        }

        private string GetGeneratedName()
        {
            string name = string.Empty;
            if (repeater.UIElementFullPath.LastIndexOf(".") > 0)
                name = repeater.UIElementFullPath.Substring(repeater.UIElementFullPath.LastIndexOf(".") + 1, repeater.UIElementFullPath.Length - repeater.UIElementFullPath.LastIndexOf(".") - 1);
            else
                name = repeater.UIElementFullPath;
            return name;
        }
    }
}
