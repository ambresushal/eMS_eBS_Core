using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.repository.extensions
{
    public static class ObjectVersionAttribXrefRepository
    {
        #region Private Memebers

        #endregion Private Members

        #region Public Properties

        #endregion Public Properties

        #region Constructor

        #endregion Constructor

        #region Public Methods
        public static bool IsObjectVersionAttribXrefExists(this IRepositoryAsync<ObjectVersionAttribXref> objectVersionAttribXrefRepository, int objectDefinitionID, int objectVersionID, int attributeID)
        {
            return objectVersionAttribXrefRepository
                             .Query()
                             .Filter(c => c.OID == objectDefinitionID && c.VersionID == objectVersionID && c.AttrID == attributeID)
                             .Get()
                             .Any();
        }

        #endregion Public Methods

        #region Private Methods

        #endregion Private Methods
    }
}
