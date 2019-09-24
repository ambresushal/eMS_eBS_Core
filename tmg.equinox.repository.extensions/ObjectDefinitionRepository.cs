using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.repository.extensions
{
    public static class ObjectDefinitionRepository
    {
        #region Private Memebers

        #endregion Private Members

        #region Public Properties

        #endregion Public Properties

        #region Constructor

        #endregion Constructor

        #region Public Methods
        public static bool IsObjectDefinitionExists(this IRepositoryAsync<ObjectDefinition> objectDefinitionRepository, string objectName, int tenantID, bool isLocked)
        {
            return objectDefinitionRepository
                             .Query()
                             .Filter(c => c.ObjectName == objectName && c.TenantID == tenantID && c.Locked == isLocked)
                             .Get()
                             .Any();
        }
        #endregion Public Methods

        #region Private Methods

        #endregion Private Methods
    }
}
