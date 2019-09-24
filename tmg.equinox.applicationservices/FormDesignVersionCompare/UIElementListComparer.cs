
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
    internal class UIElementListComparer : VersionComparer
    {
        int formDesignVersionId;
        int previousFormDesignVersionId;
        IUnitOfWorkAsync _unitOfWork;

        internal UIElementListComparer(int formDesignVersionId,int previousFormDesignVersionId, IUnitOfWorkAsync unitOfWork)
        {
            this.formDesignVersionId = formDesignVersionId;
            this.previousFormDesignVersionId = previousFormDesignVersionId;
            this._unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Compares two FormDesignVersions and returns true if UI ELements have been added or removed 
        /// in the new form design version
        /// Elements that have been replaced are not compared (compare replaced elements by Element Name in the UIElement table)
        /// </summary>
        /// <returns></returns>
        internal override bool IsMajorVersion()
        {
            bool isMajor = false;
            List<string> previousElementNames = (from elem in this._unitOfWork.RepositoryAsync<UIElement>().Get()
                                       join elemMap in this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Get()
                                       on elem.UIElementID equals elemMap.UIElementID
                                       where elemMap.FormDesignVersionID == this.previousFormDesignVersionId
                                       select elem.UIElementName).ToList();

            List<string> elementNames = (from elem in this._unitOfWork.RepositoryAsync<UIElement>().Get()
                                       join elemMap in this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Get()
                                       on elem.UIElementID equals elemMap.UIElementID
                                       where elemMap.FormDesignVersionID == this.formDesignVersionId
                                       select elem.UIElementName).ToList();
       

            if (previousElementNames.Except(elementNames).Count() > 0 || elementNames.Except(previousElementNames).Count() > 0)
            {
                isMajor = true;
            }

           
            return isMajor;
        }
    }
}
