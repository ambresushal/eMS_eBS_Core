using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.PBPImport;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IPBPMappingServices
    {
        void InitializeVariables(IUnitOfWorkAsync unitOfWorkAsync);

        IEnumerable<PBPMappingViewModel> GetPBPViewMapList();

        IEnumerable<PBPMappingViewModel> GetMapping(string TableName, bool isFullMigration, string MappingType,int planYear);
    }
}
