using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.applicationservices.FormDesignDetail
{
    internal class ValidationDesignBuilder
    {
        UIElement element;
        IEnumerable<UIElement> formElementList;
        List<Validator> ValidatorList;
        List<RegexLibrary> regexLibraryList;
        List<ApplicationDataType> elementDataTypeList;
        internal ValidationDesignBuilder(UIElement element, IEnumerable<UIElement> formElementList, List<Validator> validatorList, List<RegexLibrary> regexLibraryList, List<ApplicationDataType> elementDataTypeList)
        {
            this.element = element;
            this.formElementList = formElementList;
            this.ValidatorList = validatorList;
            this.regexLibraryList = regexLibraryList;
            this.elementDataTypeList = elementDataTypeList;
        }

        /// <summary>
        /// SH Updated the method on 12/10/2014 : Setting the field 'MaskExpression'
        /// SH Updated the method on 12/15/2014 : Setting the field 'Placeholder' & 'LibraryRegexName'
        /// </summary>
        /// <param name="fullParentName"></param>
        /// <returns></returns>
        internal ValidationDesign BuildElement(string fullParentName)
        {
            ValidationDesign design = new ValidationDesign();
           
            design = (from v in ValidatorList
                      where v.UIElementID == element.UIElementID
                      select new ValidationDesign
                      {
                          FullName = fullParentName + "." + element.GeneratedName,
                          UIElementName = element.UIElementName,
                          IsRequired = v.IsRequired,
                          IsError = false,
                          Regex = v.IsLibraryRegex.HasValue ? (v.IsLibraryRegex.Value ? regexLibraryList.Where(c => c.LibraryRegexID == v.LibraryRegexID).Select(c => c.RegexValue).FirstOrDefault() : v.Regex) : string.Empty,
                          ValidationMessage= v.IsLibraryRegex.HasValue ? (v.IsLibraryRegex.Value ? regexLibraryList.Where(c => c.LibraryRegexID == v.LibraryRegexID).Select(c => c.Message).FirstOrDefault() : v.Message) : string.Empty,

                          HasMaxLength = element is TextBoxUIElement ? ((TextBoxUIElement)element).MaxLength > 0 : false,
                          MaxLength = element is TextBoxUIElement ? ((TextBoxUIElement)element).MaxLength : 0,
                          DataType = GetUIElementDataType(element),
                          IsActive = true,
                          MaskExpression = v.IsLibraryRegex.HasValue ? (v.IsLibraryRegex.Value ? regexLibraryList.Where(c => c.LibraryRegexID == v.LibraryRegexID).Select(c => c.MaskExpression).FirstOrDefault() : v.MaskExpression) : string.Empty,
                          LibraryRegexName = v.IsLibraryRegex.HasValue ? (v.IsLibraryRegex.Value ? regexLibraryList.Where(c => c.LibraryRegexID == v.LibraryRegexID).Select(c => c.LibraryRegexName).FirstOrDefault() : string.Empty) : string.Empty,
                          PlaceHolder = v.IsLibraryRegex.HasValue ? (v.IsLibraryRegex.Value ? regexLibraryList.Where(c => c.LibraryRegexID == v.LibraryRegexID).Select(c => c.Placeholder).FirstOrDefault() : string.Empty) : string.Empty,
                          MaskFlag = v.MaskFlag 
                      }).FirstOrDefault();

            return design;
        }

        private string GetUIElementDataType(UIElement element)
        {
            string dataType = string.Empty;
            var desc = (from dType in elementDataTypeList
                        where dType.ApplicationDataTypeID == element.UIElementDataTypeID
                        && (element is TextBoxUIElement  || element is CalendarUIElement)
                        select dType.ApplicationDataTypeName).FirstOrDefault();
            if (desc != null)
            {
                dataType = desc;
            }
            return dataType;
        }

       
    }
}
