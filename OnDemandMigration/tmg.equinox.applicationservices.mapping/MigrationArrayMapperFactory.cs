using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.pbp
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
