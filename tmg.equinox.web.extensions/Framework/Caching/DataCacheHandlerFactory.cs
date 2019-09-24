using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace tmg.equinox.web.Framework.Caching
{
    public class DataCacheHandlerFactory
    {
        public static IDataCacheHandler GetHandler()
        {
            IDataCacheHandler handler = null;

            string useCompression = ConfigurationManager.AppSettings["EnableDataCompression"].ToString();

            if (string.Equals(useCompression, "true", StringComparison.OrdinalIgnoreCase))
            {
                handler = new FormInstanceCompressedDataCacheHandler();
            }
            else
            {
                handler = new FormInstanceDataCacheHandler();
            }

            return handler;
        }
    }
}