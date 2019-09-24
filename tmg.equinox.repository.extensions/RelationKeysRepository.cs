using System.Linq;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.repository.extensions
{
    public static class RelationKeysRepository
    {
        #region Private Memebers

        #endregion Private Members

        #region Public Properties

        #endregion Public Properties

        #region Constructor

        #endregion Constructor

        #region Public Methods

        public static bool IsRelationKeyExists(this IRepositoryAsync<RelationKey> objectTreeRepository, int lhsAttributeId, int rhsAttributeId, int relationId)
        {
            return objectTreeRepository
                             .Query()
                             .Filter(c => c.LHSAttrID == lhsAttributeId && c.RHSAttrID == rhsAttributeId && c.RelationID == relationId)
                             .Get()
                             .Any();
        }

        #endregion Public Methods

        #region Private Methods

        #endregion Private Methods
    }
}
