using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.ServiceDesign.ServiceDesignVersion;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.identitymanagement;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.applicationservices.ServiceDesignDetail
{
    internal class SectionDesignBuilder
    {
        ServiceDefinition section;
        IEnumerable<UIElement> formElementList;
        List<DataSourceDesign> dataSourceList;
        IEnumerable<ServiceDefinition> serviceDefinitionList;
        IUnitOfWorkAsync _unitOfWork;
        SectionUIElement sectionUIElement;
        tmg.equinox.applicationservices.FormDesignDetail.MasterLists msList;

        internal SectionDesignBuilder(ServiceDefinition section, IEnumerable<ServiceDefinition> serviceDefinitionList, SectionUIElement sectionUIElement, IEnumerable<UIElement> formElementList, List<DataSourceDesign> dataSourceList, tmg.equinox.applicationservices.FormDesignDetail.MasterLists msList, IUnitOfWorkAsync unitOfWork)
        {
            this.section = section;
            this.formElementList = formElementList;
            this.dataSourceList = dataSourceList;
            this._unitOfWork = unitOfWork;
            this.serviceDefinitionList = serviceDefinitionList;
            this.sectionUIElement = sectionUIElement;
            this.msList = msList;
        }

        internal SectionDesign BuildSection(string fullParentName)
        {
            SectionDesign design = null;
            try
            {
                if (section != null)
                {
                    design = new SectionDesign();
                    design.Elements = new List<ElementDesign>();
                    design.ID = section.ServiceDefinitionID;
                    design.UIElementID = section.UIElementID;
                    design.DisplayName = section.DisplayName;
                    design.GeneratedName = GetGeneratedName();
                    design.UIElementFullName = ServiceDesignBuilder.GetUIElementFullPath(sectionUIElement, formElementList.ToList());

                    GetChildElementDesigns(design, "parent");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return design;
        }

        private void GetChildElementDesigns(SectionDesign design, string isParent)
        {
            if (serviceDefinitionList != null)
            {
                var serviceDefElements = from t in section.ServiceDefinitions
                                         join u in serviceDefinitionList
                                         on t.ServiceDefinitionID equals u.ServiceDefinitionID
                                         orderby t.Sequence
                                         select t;

                if (serviceDefElements != null && serviceDefElements.Count() > 0)
                {
                    foreach (var elem in serviceDefElements)
                    {
                        UIElement element = formElementList.Where(c => c.UIElementID == elem.UIElementID).FirstOrDefault();
                        if (element != null)
                        {
                            {
                                UIElementDesignBuilder builder = new UIElementDesignBuilder(elem, serviceDefinitionList, element, formElementList, dataSourceList, msList, _unitOfWork);
                                design.Elements.Add(builder.BuildElement(design.UIElementFullName));
                            }
                        }
                    }
                }
            }
        }

        private string GetGeneratedName()
        {
            string name = string.Empty;
            if (section.UIElementFullPath.LastIndexOf(".") > 0)
                name = section.UIElementFullPath.Substring(section.UIElementFullPath.LastIndexOf(".") + 1, section.UIElementFullPath.Length - section.UIElementFullPath.LastIndexOf(".") - 1);
            else
                name = section.UIElementFullPath;
            return name;
        }
    }
}
