using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.repository.extensions
{
    public static class ObjectTreeRepository
    {
        #region Private Memebers

        #endregion Private Members

        #region Public Properties

        #endregion Public Properties

        #region Constructor

        #endregion Constructor

        #region Public Methods
        public static bool IsObjectTreeExists(this IRepositoryAsync<ObjectTree> objectTreeRepository, int parentObjectID, int rootObjectID, int relationID, int versionID)
        {
            return objectTreeRepository
                             .Query()
                             .Filter(c => c.ParentOID == parentObjectID && c.RootOID == rootObjectID && c.RelationID == relationID && c.VersionID == versionID)
                             .Get()
                             .Any();
        }

        #endregion Public Methods

        #region Private Methods

        #endregion Private Methods
    }
}
