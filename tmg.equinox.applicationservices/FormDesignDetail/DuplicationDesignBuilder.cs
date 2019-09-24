using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.applicationservices.FormDesignDetail
{
    internal class DuplicationDesignBuilder
    {
        UIElement element;
        IEnumerable<UIElement> formElementList;
        List<UIElement> DuplicatorList;
        List<RegexLibrary> regexLibraryList;
        List<ApplicationDataType> elementDataTypeList;

        internal DuplicationDesignBuilder(UIElement element, IEnumerable<UIElement> formElementList, List<UIElement> duplicatorList, List<RegexLibrary> regexLibraryList, List<ApplicationDataType> elementDataTypeList)
        {
            this.element = element;
            this.formElementList = formElementList;
            this.DuplicatorList = duplicatorList;
            this.regexLibraryList = regexLibraryList;
            this.elementDataTypeList = elementDataTypeList;
        }

        internal DuplicationDesign BuildElement(string fullParentName)
        {
            DuplicationDesign design = new DuplicationDesign();

            design = (from d in DuplicatorList
                      where d.UIElementID == element.UIElementID
                      select new DuplicationDesign
                      {
                          FullName = fullParentName + "." + element.GeneratedName,
                          UIElementName = element.UIElementName,
                          GeneratedName = element.GeneratedName,
                          Type = GetElementType(element),
                          ParentUIElementName = fullParentName,
                          ParentUIElementID = d.ParentUIElementID,
                          CheckDuplicate = d.CheckDuplicate
                      }).FirstOrDefault();

            return design;
        }

        private string GetElementType(UIElement uielement)
        {
            string uIElementType = string.Empty;
            try
            {
                if (uielement is RadioButtonUIElement)
                {
                    uIElementType = ElementTypes.list[0];
                }
                else if (uielement is CheckBoxUIElement)
                {
                    uIElementType = ElementTypes.list[1];
                }
                else if (uielement is TextBoxUIElement)
                {
                    switch (((TextBoxUIElement)uielement).UIElementTypeID)
                    {
                        case 3:
                            uIElementType = ElementTypes.list[2];
                            break;
                        case 4:
                            uIElementType = ElementTypes.list[3];
                            break;
                        case 10:
                            uIElementType = ElementTypes.list[9];
                            break;
                        case 11:
                            uIElementType = ElementTypes.list[10];
                            break;
                        case 13:
                            uIElementType = ElementTypes.list[12];
                            break;
                    }
                }
                else if (uielement is DropDownUIElement)
                {
                    switch (((DropDownUIElement)uielement).UIElementTypeID)
                    {
                        case 5:
                            uIElementType = ElementTypes.list[4];
                            break;
                        case 12:
                            uIElementType = ElementTypes.list[11];
                            break;
                    }
                }
                else if (uielement is CalendarUIElement)
                {
                    uIElementType = ElementTypes.list[5];
                }
                else if (uielement is SectionUIElement)
                {
                    uIElementType = ElementTypes.list[8];
                }
                else if (uielement is RepeaterUIElement)
                {
                    uIElementType = ElementTypes.list[6];
                }
                else if (uielement is TabUIElement)
                {
                    uIElementType = ElementTypes.list[7];
                }
                else
                {
                    uIElementType = "-";
                }

            }
            catch (Exception ex)
            {
                throw;
            }
            return uIElementType;
        }

    }
}
