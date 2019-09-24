using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.web.ODMExecuteManager.Interface;

namespace tmg.equinox.web.ODMExecuteManager
{
    public class MigrationArrayMapperFactory
    {
        public IMigrationArrayMapper GetMapper(string mappingType)
        {
            IMigrationArrayMapper mapper;
            switch (mappingType)
            {
                default:
                    mapper = new MigrationArrayDefaultMapper();
                    break;
            }
            return mapper;
        }
    }
}
