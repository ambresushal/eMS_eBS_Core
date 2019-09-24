using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.repository.extensions
{
    public static class FormDesignVersionRepository
    {
        #region Private Memebers

        #endregion Private Members

        #region Public Properties

        #endregion Public Properties

        #region Constructor

        #endregion Constructor

        #region Public Methods
        public static bool HasFinalizedFormDesignVersions(this IRepositoryAsync<FormDesignVersion> formDesignVersionRepository, int formDesignID)
        {
            int statusId = Convert.ToInt32(tmg.equinox.domain.entities.Status.Finalized);
            return formDesignVersionRepository
                             .Query()
                             .Filter(c => c.FormDesignID == formDesignID)
                             .Include(c => c.Status)
                             .Get()
                             .Where(c => c.StatusID == statusId)
                             .Any();
        }

        public static bool IsFormDesignVersionFinalized(this IRepositoryAsync<FormDesignVersion> formDesignVersionRepository, int formDesignVersionID)
        {
            int statusId = Convert.ToInt32(tmg.equinox.domain.entities.Status.Finalized);
            return formDesignVersionRepository
                             .Query()
                             .Filter(c => c.FormDesignVersionID == formDesignVersionID)
                             .Include(c => c.Status)
                             .Get()
                             .Where(c => c.StatusID == statusId)
                             .Any();
        }
        public static bool IsFormDesignVersionUsedInFormInstance(this IRepositoryAsync<FormInstance> FormInstanceRepository, int formDesignID, int formDesignVersionID)
        {
            return FormInstanceRepository
                             .Query()
                             .Filter(c => c.FormDesignID ==formDesignID && c.FormDesignVersionID == formDesignVersionID)
                             .Get()
                             .Any();
        }
        #endregion Public Methods

        #region Private Methods

        #endregion Private Methods
    }
}
