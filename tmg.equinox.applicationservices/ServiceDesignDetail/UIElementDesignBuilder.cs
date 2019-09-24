using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.ServiceDesign.ServiceDesignVersion;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.applicationservices.ServiceDesignDetail
{
    internal class UIElementDesignBuilder
    {
        ServiceDefinition serviceElement;
        IEnumerable<ServiceDefinition> serviceDefinitionList;
        UIElement uielement;
        IEnumerable<UIElement> formElementList;
        List<DataSourceDesign> dataSourceList;
        IUnitOfWorkAsync _unitOfWork;
        tmg.equinox.applicationservices.FormDesignDetail.MasterLists msList;

        internal UIElementDesignBuilder(ServiceDefinition serviceElement, IEnumerable<ServiceDefinition> serviceDefinitionList, UIElement uielement, IEnumerable<UIElement> formElementList, List<DataSourceDesign> dataSourceList, tmg.equinox.applicationservices.FormDesignDetail.MasterLists msList, IUnitOfWorkAsync unitOfWork)
        {
            this.serviceDefinitionList = serviceDefinitionList;
            this.serviceElement = serviceElement;
            this.formElementList = formElementList;
            this.dataSourceList = dataSourceList;
            this._unitOfWork = unitOfWork;
            this.uielement = uielement;
            this.formElementList = formElementList;
            this.msList = msList;
        }

        internal ElementDesign BuildElement(string fullParentName)
        {
            ElementDesign design = new ElementDesign();
            design.UIElementID = serviceElement.UIElementID;
            design.GeneratedName = GetGeneratedName();
            design.UIElementFullName = ServiceDesignBuilder.GetUIElementFullPath(uielement, formElementList.ToList());
            design.DisplayName = serviceElement.DisplayName;
            design.IsRequired = serviceElement.IsRequired;
            design.IsKey = serviceElement.IsKey;
            design.Type = GetUIElementType();
            design.DataType = GetUIElementDataType();
            design.DataTypeID = serviceElement.UIElementDataTypeID;
            design.Repeater = uielement is RepeaterUIElement ? GetRepeaterDesign(fullParentName) : null;
            design.Section = uielement is SectionUIElement ? GetSectionDesign(fullParentName) : null;
            design.IsSameSectionRuleSource = uielement.IsSameSectionRuleSource;

            return design;
        }

        private RepeaterDesign GetRepeaterDesign(string fullParentName)
        {
            RepeaterDesignBuilder builder = new RepeaterDesignBuilder(serviceElement, serviceDefinitionList, uielement as RepeaterUIElement, formElementList, dataSourceList, msList, _unitOfWork);
            RepeaterDesign design = builder.BuildRepeater(fullParentName);
            return design;
        }

        private SectionDesign GetSectionDesign(string fullParentName)
        {
            SectionDesignBuilder builder = new SectionDesignBuilder(serviceElement, serviceDefinitionList, uielement as SectionUIElement, formElementList, dataSourceList, msList, _unitOfWork);
            SectionDesign design = builder.BuildSection(fullParentName);
            return design;
        }

        private string GetUIElementDataType()
        {
            string dataType = string.Empty;
            var desc = (from dType in msList.ElementDataTypes
                        where dType.ApplicationDataTypeID == serviceElement.UIElementDataTypeID
                        select dType.ApplicationDataTypeName).FirstOrDefault();
            if (desc != null)
            {
                dataType = desc;
            }
            return dataType;
        }


        private string GetUIElementType()
        {
            string uiElementType = string.Empty;

            uiElementType = msList.ElementTypes
                                .Where(c => c.UIElementTypeID == serviceElement.UIElementTypeID)
                                .Select(c => c.Description)
                                .SingleOrDefault();
            return uiElementType;
        }

        private string GetGeneratedName()
        {
            string name = string.Empty;
            if (serviceElement.UIElementFullPath.LastIndexOf(".") > 0)
                name = serviceElement.UIElementFullPath.Substring(serviceElement.UIElementFullPath.LastIndexOf(".") + 1, serviceElement.UIElementFullPath.Length - serviceElement.UIElementFullPath.LastIndexOf(".") - 1);
            else
                name = serviceElement.UIElementFullPath;
            return name;
        }
    }
}
