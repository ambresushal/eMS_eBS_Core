using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.dependencyresolution;
using tmg.equinox.infrastructure.exceptionhandling;

namespace tmg.equinox.services.genericWebApi.Areas.HelpPage.Models
{
    public class Products
    {
        private int tenantId = 1;
        public int ID { get; set; }
        public string Name { get; set; }
        public string Data { get; set; }
        public string DesignTemplate { get; set; }
        public string DesignTempateVersion { get; set; }
        private IFolderVersionServices _serviceFolder { get; set; }

        public Products()
        {
            _serviceFolder = UnityConfig.Resolve<IFolderVersionServices>();
        }

        public List<Products> GetProductById(int Id)
        {
            try
            {
            var lstProducts = (from fd in this._serviceFolder.GetProductById(Id)
                               select new Products
                               {
                                   ID = Id,
                                   Name=fd.FormInstanceName,
                                   DesignTemplate=fd.FormDesignName,
                                   DesignTempateVersion=fd.FormDesignVersionNumber,
                                   Data = fd.FormData,
                               }).ToList();
            return lstProducts;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                return null;
            }
        }

        public List<Products> GetProductByProductId(string productId)
        {
            try
            {
                var lstProducts = (from fd in this._serviceFolder.GetProductByProductId(productId)
                                   select new Products
                                   {
                                       //PID = productId,
                                       Data = fd.FormData,
                                   }).ToList();
                return lstProducts;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                return null;
            }
        }
        public List<Products> GetProductBySecions(int productId, string sections)
        {
            try
            {
            var lstProducts = (from fd in this._serviceFolder.GetProductBySecionName(productId, sections)
                               select new Products
                               {
                                   Data=fd.FormData,
                                   DesignTemplate=fd.FormDesignName,
                                   DesignTempateVersion=fd.FormDesignVersionNumber,
                                   ID=productId,
                                   Name=fd.FormInstanceName
                               }).ToList();
            return lstProducts;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                return null;
            }
        }
    }
    public class url
    {
        public string products { get; set; }
        public string rowTemplate { get; set; }
    }
}