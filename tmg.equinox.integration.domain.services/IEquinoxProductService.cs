using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels;

namespace tmg.equinox.integration.domain.services
{
    public interface IEquinoxProductService
    {
        /// <summary>
        /// <para>Gets Json for a product</para>
        /// </summary>     
        /// <returns><para>product id (forminstance id)</para></returns>    
        EquinioxProduct GetProductJson(int productid);


        /// <summary>
        /// <para>Gets Json for a masterlist</para>
        /// </summary>     
        /// <returns><para>masterlit id (forminstance id)</para></returns>  
        dynamic GetMasterListJson(int masterlistid);



        /// <summary>
        /// <para>Gets all product for a plugin and version</para>
        /// </summary>     
        /// <returns><para>masterlit id (forminstance id)</para></returns>  
        List<PDPD481Model> GetAllProducts(string plugin, string version);

        int GetMasterListInstanceId(int folderVersionId);
    }
}
