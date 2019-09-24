using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.repository.extensions
{
    public static class ObjectVersionRepository
    {
        #region Private Memebers

        #endregion Private Members

        #region Public Properties

        #endregion Public Properties

        #region Constructor

        #endregion Constructor

        #region Public Methods
        public static bool IsVersionExists(this IRepositoryAsync<ObjectVersion> objectVersionRepository, string versionName)
        {
            return objectVersionRepository
                             .Query()
                             .Filter(c => c.VersionName == versionName)
                             .Get()
                             .Any();
        }

        public static ObjectVersion FindByName(this IRepositoryAsync<ObjectVersion> objectVersionRepository, string versionName)
        {
            return objectVersionRepository
                             .Query()
                             .Filter(c => c.VersionName == versionName)
                             .Get()
                             .SingleOrDefault();
        }
        #endregion Public Methods

        #region Private Methods

        #endregion Private Methods
    }
}
