
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

namespace tmg.equinox.applicationservices.FormDesignVersionCompare
{
    internal class UIElementDetailComparer : VersionComparer
    {
        int formDesignVersionId;
        int previousFormDesignVersionId;
        IUnitOfWorkAsync _unitOfWork;

        internal UIElementDetailComparer(int formDesignVersionId, int previousFormDesignVersionId, IUnitOfWorkAsync unitOfWork)
        {
            this.formDesignVersionId = formDesignVersionId;
            this.previousFormDesignVersionId = previousFormDesignVersionId;
            this._unitOfWork = unitOfWork;
        }
        /// <summary>
        /// Compare elements that have the same name but a new element has been created for the new version due to changes to the properties of the element
        /// To classify as a Major Version - check for Label or Rule change, Check for LoadFromServer property change(true)
        /// </summary>
        /// <returns></returns>
        internal override bool IsMajorVersion()
        {
            bool isMajor = false;
            List<UIElementMap> elements = (from elem in this._unitOfWork.RepositoryAsync<UIElement>().Get()
                                           join elemMap in this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Get()
                                           on elem.UIElementID equals elemMap.UIElementID
                                           join repeaterUIElement in this._unitOfWork.RepositoryAsync<RepeaterUIElement>().Get()
                                           on elem.UIElementID equals repeaterUIElement.UIElementID
                                           into repeaterElement
                                           from repeaterElementList in repeaterElement.DefaultIfEmpty()
                                           where elemMap.FormDesignVersionID == this.formDesignVersionId
                                           select new UIElementMap
                                           {
                                               FormDesignVersionId = elemMap.FormDesignVersionID,
                                               ElementName = elem.UIElementName,
                                               UIElementId = elem.UIElementID,
                                               LoadFromServer = elem is RepeaterUIElement ? repeaterElementList.LoadFromServer : false
                                           }).ToList();

            

            List<UIElementMap> previousElements = (from elem in this._unitOfWork.RepositoryAsync<UIElement>().Get()
                                                   join elemMap in this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Get()
                                                   on elem.UIElementID equals elemMap.UIElementID
                                                   join repeaterUIElement in this._unitOfWork.RepositoryAsync<RepeaterUIElement>().Get()
                                                 on elem.UIElementID equals repeaterUIElement.UIElementID
                                                 into repeaterElement
                                                   from repeaterElementList in repeaterElement.DefaultIfEmpty()
                                                   where elemMap.FormDesignVersionID == this.previousFormDesignVersionId
                                                   select new UIElementMap
                                                   {
                                                       FormDesignVersionId = elemMap.FormDesignVersionID,
                                                       ElementName = elem.UIElementName,
                                                       UIElementId = elem.UIElementID,
                                                       LoadFromServer = elem is RepeaterUIElement ? repeaterElementList.LoadFromServer : false
                                                   }).ToList();

            
            List<UIElementIDMap> uiElementMapList = (from elem in elements
                                                     join prevElem in previousElements
                                                     on elem.ElementName equals prevElem.ElementName
                                                     where elem.UIElementId != prevElem.UIElementId
                                                     select new UIElementIDMap
                                                     {
                                                         UIElementId = elem.UIElementId,
                                                         PreviousUIElementId = prevElem.UIElementId
                                                     }).ToList();


            if (uiElementMapList != null && uiElementMapList.Count() > 0)
            {
                IList<UIElementMap> newUIElementIdList = elements.Where(p => !previousElements.Any(p2 => p2.UIElementId == p.UIElementId)).ToList();

                if (newUIElementIdList.Count > 0)
                {
                    if (newUIElementIdList.Any(p => p.LoadFromServer).Equals(true))
                    {
                        isMajor = true;
                    }
                }
                foreach (var uiElemMap in uiElementMapList)
                {

                    //check for label/custom rule change
                    UIElementPropertyComparer propComparer = new UIElementPropertyComparer(uiElemMap.UIElementId, uiElemMap.PreviousUIElementId, _unitOfWork);
                    isMajor = propComparer.IsMajorVersion();
                    if (isMajor == true)
                    {
                        break;
                    }
                    UIElementRuleComparer ruleComparer = new UIElementRuleComparer(uiElemMap.UIElementId, uiElemMap.PreviousUIElementId, _unitOfWork);
                    isMajor = ruleComparer.IsMajorVersion();
                    if (isMajor == true)
                    {
                        break;
                    }
                }
            }
            else
            {

                IList<UIElementIDMap> loadFromServerEnabledUIElementIdList = (from elem in elements
                                                                              join prevElem in previousElements
                                                          on elem.ElementName equals prevElem.ElementName
                                                                              where elem.LoadFromServer != prevElem.LoadFromServer
                                                                              select new UIElementIDMap
                                                                              {
                                                                                  UIElementId = elem.UIElementId,
                                                                                  PreviousUIElementId = prevElem.UIElementId
                                                                              }).ToList();
                if (loadFromServerEnabledUIElementIdList.Count > 0)
                {
                    isMajor = true;
                }
            }
            return isMajor;
        }

        private class UIElementMap
        {
            internal int FormDesignVersionId { get; set; }
            internal string ElementName { get; set; }
            internal int UIElementId { get; set; }
            internal bool LoadFromServer { get; set; }

        }

        private class UIElementIDMap
        {
            internal int UIElementId { get; set; }
            internal int PreviousUIElementId { get; set; }
        }
    }
}
