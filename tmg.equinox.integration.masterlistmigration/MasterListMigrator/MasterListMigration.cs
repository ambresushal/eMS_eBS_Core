using Newtonsoft.Json;
using System;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.dependencyresolution;
using tmg.equinox.infrastructure.exceptionhandling;

namespace tmg.equinox.integration.migration
{
    public class MasterListMigration
    {
        private IFacetImportService _importService;

        public MasterListMigration()
        {
            _importService = UnityConfig.Resolve<IFacetImportService>(); ;
        }

        public bool ExecuteMasterListMigration()
        {
            bool status = true;
            try
            {
                ServiceResult result = new ServiceResult();
                _importService.ExecuteMasterListMigration();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                status = false;
                if (reThrow)
                    throw ex;
            }
            return status;
        }
    }
}
