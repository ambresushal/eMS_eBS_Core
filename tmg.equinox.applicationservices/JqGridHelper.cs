using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.infrastructure.util;
using tmg.equinox.repository.extensions;

namespace tmg.equinox.applicationservices
{
    public class JqGridHelper
    {
        public static SearchCriteria GetCriteria(string jsonData)
        {
            try
            {
                SearchCriteria criteria = new SearchCriteria();
                if (!string.IsNullOrEmpty(jsonData))
                {
                    criteria = JsonHelper.JsonDeserialize<SearchCriteria>(jsonData);
                }
                return criteria;
            }
            catch
            {
                return null;
            }
        }

    }
}
