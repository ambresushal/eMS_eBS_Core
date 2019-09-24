using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.repository.extensions
{
    public static class ServiceDesignVersionRepository
    {
        public static bool IsFinalized(this IRepositoryAsync<ServiceDesignVersion> repository, int serviceDesignVersionID)
        {
            return repository.Query()
                                .Filter(c => c.IsFinalized == true && c.ServiceDesignVersionID == serviceDesignVersionID)
                                .Get()
                                .Any();

        }
    }
}
