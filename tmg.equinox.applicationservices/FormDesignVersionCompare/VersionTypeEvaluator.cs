
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
    internal class VersionTypeEvaluator
    {
        int formDesignId;
        int formDesignVersionId;
        IUnitOfWorkAsync _unitOfWork;

        internal VersionTypeEvaluator(int formDesignId, int formDesignVersionId, IUnitOfWorkAsync unitOfWork)
        {
            this.formDesignId = formDesignId;
            this.formDesignVersionId = formDesignVersionId;
            this._unitOfWork = unitOfWork;
        }

        internal bool IsMajorVersion() 
        {
            bool isMajor = false;
            //get previous version
            int previousFormDesignVersionId = GetPreviousVersionId();
            VersionComparer comparer = new UIElementListComparer(this.formDesignVersionId, previousFormDesignVersionId, _unitOfWork);
            isMajor = comparer.IsMajorVersion();
            if (isMajor == false) 
            {
                comparer = new UIElementDetailComparer(this.formDesignVersionId, previousFormDesignVersionId, _unitOfWork);
                isMajor = comparer.IsMajorVersion();
            }
            //First version when finalized  
            if (previousFormDesignVersionId <= 0)
            {
                isMajor = false;
            }
            return isMajor;
        }

        private int GetPreviousVersionId() 
        { 
            int previousFormDesignVersionId = (from fdv in this._unitOfWork.RepositoryAsync<FormDesignVersion>()
                                                .Query()
                                                .Filter(d => d.FormDesignID == this.formDesignId && d.StatusID == 3)
                                                .Get().OrderByDescending(e =>e.EffectiveDate) select fdv.FormDesignVersionID).FirstOrDefault();
            return previousFormDesignVersionId;
        }

        private string GetNextVersion(bool isMajor)
        {
            return "";
        }
    }
}
