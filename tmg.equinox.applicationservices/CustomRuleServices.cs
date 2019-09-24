using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.applicationservices
{
    public partial class CustomRuleServices:ICustomRuleService
    {
         #region Private Memebers
       
        private IFolderVersionServices _FolderVersionServices { get; set; }
        
        #endregion Private Members

        #region Public Properties

        #endregion Public Properties

        #region Constructor
        public CustomRuleServices(IFolderVersionServices folderVersionServices)
        {
            this._FolderVersionServices = folderVersionServices;           
        }
        #endregion Constructor

        #region Public Methods
        public IList<object> FilterAdditionalServiceListData(IList<object> masterListServiceData, IList<object> masterListStandardServiceDetailsList)
        {
            string[] standardServiceList = new String[0];
            IList<object> additionalStandardServiceList = new List<object>();
            try
            {
                //return selected mandate service List
                IList<object> selectedServiceDetailsList = GetSelectedStandardServiceList(masterListStandardServiceDetailsList, standardServiceList);
                //return filtered service List
                additionalStandardServiceList = filterSelectedStandardServiceList(masterListServiceData, selectedServiceDetailsList);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return additionalStandardServiceList;
        }

        public IList<object> UpdateAdditionalServiceListData(string[] standardServiceList, IList<object> masterListServiceData, IList<object> masterListStandardServiceDetailsList)
        {
            IList<object> additionalStandardServiceList = new List<object>();

            try
            {
                //return selected mandate service List
                IList<object> selectedServiceDetailsList = GetSelectedStandardServiceList(masterListStandardServiceDetailsList, standardServiceList);

                //return filtered service List
                additionalStandardServiceList = filterSelectedStandardServiceList(masterListServiceData, selectedServiceDetailsList);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }

            return additionalStandardServiceList;
        }
        #endregion

        #region private Methods
        private IList<object> filterSelectedStandardServiceList(IList<object> masterListServiceData, IList<object> selectedServiceDetailsList)
        {
             IList<object> masterServiceList = new List<object>();
             foreach (dynamic serviceList in masterListServiceData)
             {
                 IDictionary<string, object> services = serviceList as IDictionary<string, object>;

                 //check is exist on selected serviceList
                 if (selectedServiceDetailsList.Count() > 0)
                 {
                     IList<object> isExistsSelectedService = selectedServiceDetailsList.Where(item =>
                     {
                         var dict = item as IDictionary<string, object>;
                         bool result = false;
                         int matchCount = 0;
                         int eleCount = 0;
                         foreach (var ent in services)
                         {
                             if (dict.ContainsKey(ent.Key) == true && ent.Key.ToString() != "RowIDProperty")
                             {
                                 eleCount++;
                                 if (dict[ent.Key] == null && ent.Value == null)
                                 {
                                     matchCount++;
                                 }
                                 else
                                 {
                                     if (dict[ent.Key] != null && ent.Value != null)
                                     {
                                         if (dict[ent.Key].ToString() == ent.Value.ToString())
                                         {
                                             matchCount++;
                                         }
                                     }
                                 }

                             }
                         }
                         if (matchCount == eleCount)
                         {
                             result = true;
                         }
                         return result;
                     }).ToList();

                     if (isExistsSelectedService.Count() == 0)
                     {
                         masterServiceList.Add(services);
                     }
                 }
                 else
                 {
                     masterServiceList.Add(services);

                 }
             }

             return masterServiceList;           
        }

        private IList<object> GetSelectedStandardServiceList(IList<object> masterListStandardServiceDetailsList, string[] standardServiceList)
        {
            IList<object> standardServiceDetailsList = new List<object>();
            foreach (dynamic serviceList in masterListStandardServiceDetailsList)
            {
                IDictionary<string, object> networksData = serviceList as IDictionary<string, object>;
                if (standardServiceList.Count() == 0 || (standardServiceList.Contains(networksData["MandateName"].ToString())))
                {
                    foreach (dynamic element in networksData)
                    {

                        if (networksData[element.Key] is System.Collections.Generic.IList<Object>)
                        {
                            IList<object> medicalServicesList = networksData[element.Key] as IList<object>;
                            foreach (dynamic services in medicalServicesList)
                            {
                                IDictionary<string, object> standardService = services as IDictionary<string, object>;
                                if (standardServiceDetailsList.Count() == 0)
                                {
                                    standardServiceDetailsList.Add(standardService);
                                }
                                else
                                {
                                    IList<object> isExists = standardServiceDetailsList.Where(item =>
                                    {
                                        var dict = item as IDictionary<string, object>;
                                        bool result = false;
                                        int matchCount = 0;
                                        int eleCount = 0;
                                        foreach (var ent in standardService)
                                        {
                                            if (dict.ContainsKey(ent.Key) == true && ent.Key.ToString() != "RowIDProperty")
                                            {
                                                eleCount++;
                                                if (dict[ent.Key] == null && ent.Value == null)
                                                {
                                                    matchCount++;
                                                }
                                                else
                                                {
                                                    if (dict[ent.Key] != null && ent.Value != null)
                                                    {
                                                        if (dict[ent.Key].ToString() == ent.Value.ToString())
                                                        {
                                                            matchCount++;
                                                        }
                                                    }
                                                }

                                            }
                                        }
                                        if (matchCount == eleCount)
                                        {
                                            result = true;
                                        }
                                        return result;
                                    }).ToList();
                                    if (isExists.Count() == 0)
                                    {
                                        standardServiceDetailsList.Add(standardService);
                                    }
                                }
                            }
                        }
                    }   
                }

            }

            return standardServiceDetailsList;
        }
        #endregion
    }
}
