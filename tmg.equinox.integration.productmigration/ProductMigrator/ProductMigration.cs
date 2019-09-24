using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.dependencyresolution;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.integration.domain;
using tmg.equinox.integration.facet.data.Models;

namespace tmg.equinox.integration.migration
{
    public class ProductMigrator
    {
        private IFacetProductServices _importService;
     
        public ProductMigrator()
        {
            _importService = UnityConfig.Resolve<IFacetProductServices>();
         
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool ExecuteProcess()
        {
            bool result=_importService.ExecuteProductMigration();
            return result;
        }
    }
}
