
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
    internal class UIElementPropertyComparer : VersionComparer
    {
        int uiELementId;
        int previousUIELementId;
        IUnitOfWorkAsync _unitOfWork;

        internal UIElementPropertyComparer(int uiElementId, int previousUIElementId, IUnitOfWorkAsync unitOfWork)
        {
            this.uiELementId= uiElementId;
            this.previousUIELementId= previousUIElementId;
            this._unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Compare the 2 Elements for Label change - if Labels are different, it is a Major Change
        /// </summary>
        /// <returns></returns>
        internal override bool IsMajorVersion()
        {
            bool isMajor = false;
            List<UIElement> uiElems = (from el in this._unitOfWork.RepositoryAsync<UIElement>()
                            .Query()
                            .Filter(d => d.UIElementID == uiELementId || d.UIElementID == previousUIELementId)
                            .Get()
                                       select el).ToList();
            UIElement element = uiElems.Find(d => d.UIElementID == uiELementId);
            UIElement previousElement = uiElems.Find(d => d.UIElementID == previousUIELementId);

            if (element.Label != previousElement.Label || element.CustomRule != previousElement.CustomRule) 
            {
                isMajor = true;
            }
            return isMajor;
        }
    }
}
