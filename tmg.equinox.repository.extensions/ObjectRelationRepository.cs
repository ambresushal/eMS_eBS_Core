using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.repository.extensions
{
    public static class ObjectRelationRepository
    {
        #region Private Memebers

        #endregion Private Members

        #region Public Properties

        #endregion Public Properties

        #region Constructor

        #endregion Constructor

        #region Public Methods
        public static bool IsObjectRelationExists(this IRepositoryAsync<ObjectRelation> objectRelationRepository, int relatedObjectID, string relationName, string relationNameCamelCase, string cardinality)
        {
            return objectRelationRepository
                             .Query()
                             .Filter(c => c.RelatedObjectID == relatedObjectID && c.RelationName == relationName && c.RelationNameCamelcase == relationNameCamelCase && c.Cardinality == cardinality)
                             .Get()
                             .Any();
        }

        #endregion Public Methods

        #region Private Methods

        #endregion Private Methods
    }
}
