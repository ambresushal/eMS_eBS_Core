using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.integration.Scheduler;
using tmg.equinox.integration.infrastructure.Util;
using tmg.equinox.applicationservices.viewmodels;
using Microsoft.Win32.TaskScheduler;
using tmg.equinox.integration.translator.dao.Models;
using tmg.equinox.integration.infrastructure.exceptionhandling;
using Newtonsoft.Json;
using System.Dynamic;
 using tmg.equinox.repository.interfaces;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.integration.domain.services.Impl
{
    public class EquinoxProductService : IEquinoxProductService
    {
        private IUnitOfWorkAsync _unitOfWork { get; set; }

        public EquinoxProductService()
        {
            _unitOfWork = new tmg.equinox.repository.UnitOfWork();
        }

        public EquinoxProductService(IUnitOfWorkAsync unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        /// <summary>
        /// Get  All products for a plugin and version
        /// </summary>
        /// <returns></returns>
        public List<PDPD481Model> GetAllProducts(string plugin, string version)
        {
            List<PDPD481Model> products = new List<PDPD481Model>();
            EquinioxProduct product = new EquinioxProduct();
            try
            {
                int formDesignID = Convert.ToInt32(Util.GetKeyValue("FormDesignID"));
                var formInstances = (from q in this._unitOfWork.Repository<FormInstance>()
                                                                         .Query()
                                                                         .Get()
                                                                         .Where(x => x.FormDesignID == formDesignID)
                                     select new
                                     {
                                         FormFolder = q.FolderVersion.Folder,
                                         FormInstanceId = q.FormInstanceID,
                                         FolderVersionId = q.FolderVersionID,
                                         FolderVersionNumber = q.FolderVersion.FolderVersionNumber,
                                         FormInstanceName = q.Name,
                                         JsonData = q.FormInstanceDataMaps.Where(x => x.FormInstanceID == q.FormInstanceID)
                                     }
                              ).ToList();

                if (formInstances != null)
                {
                    foreach (var frm in formInstances)
                    {
                        if (frm.JsonData != null && frm.JsonData.Count() > 0)
                        {
                            foreach (var item in frm.JsonData)
                            {
                                product = JsonConvert.DeserializeObject<EquinioxProduct>(item.FormData);

                                if (product != null && product.ProductRules != null && product.ProductRules.FacetsProductInformation != null)
                                {
                                    if (product.ProductRules.FacetsProductInformation.Plugin == plugin && product.ProductRules.FacetsProductInformation.Version == version)
                                    {
                                        PDPD481Model prod = new PDPD481Model();
                                        prod.productId = frm.FormInstanceId;
                                        prod.PDPD_ID = product.ProductRules.FacetsProductInformation.ProductID;
                                        prod.FolderVersionId = frm.FolderVersionId;
                                        prod.FolderVersionNumber = frm.FolderVersionNumber;
                                        prod.FormInstanceName = frm.FormInstanceName;
                                        prod.FolderName = frm.FormFolder.Name;

                                        products.Add(prod);
                                    }
                                }
                            }
                        }


                        //prd.JsonData = jsnstr.JsonData
                        //prd.FormInstanceName = jsnstr.FormInstances.Where(x => x.FolderVersionID == jsnstr.FolderVersionId).Select(x=>x.Name).FirstOrDefault().ToString();

                        //try
                        //{
                        //    product = ""; //JsonConvert.DeserializeObject<EquinioxProduct>(jsnstr.FormData);
                        //}
                        //catch (Exception ex)
                        //{ 
                        //  //Do Nothing
                        //}
                        //if (product != null && product.ProductRules!=null && product.ProductRules.FacetsProductInformation!=null )
                        //{
                        //    if (product.ProductRules.FacetsProductInformation.Plugin == plugin && product.ProductRules.FacetsProductInformation.Version == version)
                        //    {
                        //        PDPD481Model prod = new PDPD481Model();
                        //        prod.productId = jsnstr.FormInstanceID;
                        //        prod.PDPD_ID = product.ProductRules.FacetsProductInformation.ProductID;
                        //        products.Add(prod);
                        //    }
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return products;
        }

        /// <summary>
        /// Get  json for a product
        /// </summary>
        /// <returns></returns>
        public EquinioxProduct GetProductJson(int productid)
        {
            EquinioxProduct rootObj = new EquinioxProduct();
            try
            {
                string jsnstr = string.Empty;
                var jsonstring = (from jsn in this._unitOfWork.Repository<FormInstanceDataMap>()
                                                                         .Query()
                                                                         .Filter(jsn => jsn.FormInstanceID == productid)
                                                                         .Get()
                                  select jsn.FormData
                              ).ToList();

                if (jsonstring != null)
                {
                    jsnstr = jsonstring[0].ToString();
                    rootObj = JsonConvert.DeserializeObject<EquinioxProduct>(jsnstr);
                }
                return rootObj;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return null;
        }


        /// <summary>
        /// Get  json for a product
        /// </summary>
        /// <returns></returns>
        public dynamic GetMasterListJson(int masterlistid)
        {
            dynamic rootObj = new ExpandoObject();
            try
            {
                string jsnstr = string.Empty;
                var jsonstring = (from jsn in this._unitOfWork.Repository<FormInstanceDataMap>()
                                                                         .Query()
                                                                         .Filter(jsn => jsn.FormInstanceID == masterlistid)
                                                                         .Get()
                                  select jsn.FormData
                              ).ToList();

                if (jsonstring != null)
                {
                    jsnstr = jsonstring[0].ToString();
                    rootObj = JsonConvert.DeserializeObject(jsnstr);
                }
                return rootObj;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return null;
        }

        /// <summary>
        /// Get Master List InstanceId
        /// </summary>
        /// <param name="folderVersionId"></param>
        /// <returns></returns>
        public int GetMasterListInstanceId(int formInstanceId)
        {
            try
            {
                int folderIdForMasterList = Convert.ToInt32(Util.GetKeyValue("FolderIdForMasterList"));
                
                var prdFolderEffdate = (from ins in this._unitOfWork.Repository<FormInstance>()
                                                                       .Query()
                                                                       .Filter(x=>x.FormInstanceID==formInstanceId)
                                                                       .Get()
                                  select new
                                  { 
                                      ins.FolderVersion.EffectiveDate
                                  }
                            ).FirstOrDefault();

                DateTime prodEfectivedate = Convert.ToDateTime(prdFolderEffdate.EffectiveDate);
                var folderversioId = (from fldever in this._unitOfWork.Repository<FolderVersion>()
                                                                      .Query()
                                                                      .Filter(x => x.EffectiveDate <= prodEfectivedate && x.FolderID == folderIdForMasterList)
                                                                      .Get()
                                                                      .OrderByDescending(x=>x.EffectiveDate)
                                      select new
                                      {
                                          fldever.FolderVersionID
                                      }
                           ).FirstOrDefault();

                int fldrversioId = Convert.ToInt32(folderversioId.FolderVersionID);
                var instanceId = (from ins in this._unitOfWork.Repository<FormInstance>()
                                                                        .Query()
                                                                        .Filter(x => x.FolderVersionID == fldrversioId)
                                                                        .Get()
                                  select new
                                  {
                                      ins.FormInstanceID
                                  }
                             ).FirstOrDefault();
                 

                if (instanceId != null)
                {
                    return instanceId.FormInstanceID;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return 0;
        }
    }
}
