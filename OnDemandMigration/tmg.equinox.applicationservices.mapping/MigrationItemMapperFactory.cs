﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.pbp
{
    public class MigrationItemMapperFactory
    { 
        public IMigrationItemMapper GetMapper(string mappingType)
        {
            IMigrationItemMapper mapper;
            switch (mappingType)
            {
                case "DIRECT":
                    mapper = new MigrationItemDirectMapper();
                    break;
                case "SINGLESELECT":
                    mapper = new MigrationItemSingleSelectMapper();
                    break;
                case "MULTISELECT":
                    mapper = new MigrationItemMultiSelectMapper();
                    break;
                case "MULTIVALUECSV":
                    mapper = new MigrationItemMultiValueCSVMapper();
                    break;
                default:
                    mapper = new MigrationItemDirectMapper();
                    break;
            }
            return mapper;
        }
    }
}