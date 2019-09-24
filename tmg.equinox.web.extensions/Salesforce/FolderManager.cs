using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.dependencyresolution;

namespace tmg.equinox.web.Salesforce
{
    public class FolderManager
    {
        private IConsumerAccountService _consumerAccountService;
        private IFolderVersionServices _folderVersionService;
        public SFTemplateSettings _config;

        public FolderManager()
        {
            this._consumerAccountService = UnityConfig.Resolve<IConsumerAccountService>();
            this._folderVersionService = UnityConfig.Resolve<IFolderVersionServices>();
            this._config = SFTemplateSettings.Settings();
        }

        public void CreateFolder(JToken sfNotification)
        {
            this._consumerAccountService = UnityConfig.Resolve<IConsumerAccountService>();
            int num = 1;
            int? nullable = new int?(_config.AccountId);
            if (sfNotification["Group_Name__c"] != null)
            {
                string str = sfNotification["Group_Name__c"].ToString();
                nullable = this.GetAccountIDForGroupName(str);
            }
            int num1 = _config.FolderId;
            int num2 = _config.FolderVersionId;
            DateTime now = DateTime.Now;
            string str1 = string.Concat("Test Folder ", now.ToString("s"));
            if (sfNotification["Folder_Name__c"] != null)
            {
                str1 = sfNotification["Folder_Name__c"].ToString();
            }
            DateTime dateTime = DateTime.Now;
            if (sfNotification["Folder_Name__c"] != null)
            {
                if (!DateTime.TryParse(sfNotification["Group_Effective_Date__c"].ToString(), out dateTime))
                {
                    DateTime.TryParse("01/01/2018", out dateTime);
                }
            }
            bool flag = false;
            int num3 = _config.UserId;
            string str2 = "SalesForce Int Defined";
            int num4 = _config.MarketSegment;
            string str3 = _config.UserName;
            ServiceResult serviceResult = this._consumerAccountService.CopyFolder(num, nullable, num1, num2, str1, dateTime, flag, num3, str2, num4, 0, _config.CategoryId, "123", str3);
            if ((serviceResult.Items == null ? false : serviceResult.Items.Count<ServiceResultItem>() > 0))
            {
                ServiceResultItem serviceResultItem = serviceResult.Items.First<ServiceResultItem>();
                if ((serviceResultItem.Messages == null ? false : (int)serviceResultItem.Messages.Length > 0))
                {
                    int num5 = 0;
                    int num6 = 0;
                    int.TryParse(serviceResultItem.Messages[1], out num6);
                    int.TryParse(serviceResultItem.Messages[2], out num5);
                    if ((num5 <= 0 ? false : num6 > 0))
                    {
                        List<FormInstanceViewModel> formInstanceList = this._folderVersionService.GetFormInstanceList(num, num6, num5, 1);
                        if (sfNotification["Products__c"] != null)
                        {
                            string str4 = sfNotification["Products__c"].ToString();
                            char[] chrArray = new char[] { ';' };
                            foreach (string list in str4.Split(chrArray).ToList<string>())
                            {
                                string formInstanceName = list + " Base";
                                var formInstanceObject = formInstanceList.Where(s => s.FormDesignName != formInstanceName).FirstOrDefault();
                                if (formInstanceObject != null)
                                {
                                    int formInstanceIDForProduct = formInstanceObject.FormInstanceID;
                                    this._folderVersionService.DeleteFormInstance(num5, num, num6, formInstanceIDForProduct, _config.UserName);
                                }
                                break;
                            }
                        }

                        formInstanceList = this._folderVersionService.GetFormInstanceList(num, num6, num5, 1);
                        if ((formInstanceList == null ? false : formInstanceList.Count > 0))
                        {
                            int formInstanceID = formInstanceList[0].FormInstanceID;
                            string formInstanceData = this._folderVersionService.GetFormInstanceData(num, formInstanceID);
                            JObject jObjects = JObject.Parse(formInstanceData);
                            this.UpdateAdminFormInstanceData(jObjects, sfNotification);
                            formInstanceData = JsonConvert.SerializeObject(jObjects);
                            this._folderVersionService.SaveFormInstanceDataCompressed(formInstanceID, formInstanceData);
                        }

                    }
                }
            }
        }

        private int? GetAccountIDForGroupName(string groupName)
        {
            var accounts = _consumerAccountService.GetAccountList(1);
            int? nullable = new int?(_config.AccountId);
            string str = groupName;
            if (str != null)
            {
                var accn = accounts.Where(s => s.AccountName == str).FirstOrDefault();
                if (accn != null)
                {
                    nullable = accn.AccountID;
                }
            }
            return nullable;
        }

        private void UpdateAdminFormInstanceData(JObject formInstanceObject, JToken sfObject)
        {
            JToken item = formInstanceObject["GroupInformation"]["GeneralGroupInformation"]["GroupInformation"];
            if ((item == null ? false : sfObject != null))
            {
                if ((item["GroupName"] == null ? false : sfObject["Group_Name__c"] != null))
                {
                    item["GroupName"] = sfObject["Group_Name__c"].ToString();
                }
                if ((item["ContractingEntitysLegalName"] == null ? false : sfObject["Contracting_Entity_s_Legal_Name__c"] != null))
                {
                    item["ContractingEntitysLegalName"] = sfObject["Contracting_Entity_s_Legal_Name__c"].ToString();
                }
                if ((item["GroupEffectiveDate"] == null ? false : sfObject["Group_Effective_Date__c"] != null))
                {
                    item["GroupEffectiveDate"] = sfObject["Group_Effective_Date__c"].ToString();
                }
                if ((item["GroupRenewalDate"] == null ? false : sfObject["Group_Renewal_Date__c"] != null))
                {
                    item["GroupRenewalDate"] = sfObject["Group_Renewal_Date__c"].ToString();
                }
                if ((item["InitialTermofContract"] == null ? false : sfObject["Initial_Term_of_Contract__c"] != null))
                {
                    item["InitialTermofContract"] = sfObject["Initial_Term_of_Contract__c"].ToString();
                }
                if ((item["NumberofCoveredEmployees"] == null ? false : sfObject["Number_of_Covered_Employees__c"] != null))
                {
                    item["NumberofCoveredEmployees"] = sfObject["Number_of_Covered_Employees__c"].ToString();
                }
                if ((item["TaxID"] == null ? false : sfObject["Tax_ID__c"] != null))
                {
                    item["TaxID"] = sfObject["Tax_ID__c"].ToString();
                }
                if ((item["HAXSGroupID"] == null ? false : sfObject["HAXS_Group_ID__c"] != null))
                {
                    item["HAXSGroupID"] = sfObject["HAXS_Group_ID__c"].ToString();
                }
                if ((item["SelectGroupEntity"] == null ? false : sfObject["Select_Group_Entity__c"] != null))
                {
                    item["SelectGroupEntity"] = sfObject["Select_Group_Entity__c"].ToString();
                }
                if ((item["Doestheemployerhave50ormoreemployeeswitha75mileradiusoftheplaceofemplo"] == null ? false : sfObject["Does_the_employer_have_50_or_more_employ__c"] != null))
                {
                    item["Doestheemployerhave50ormoreemployeeswitha75mileradiusoftheplaceofemplo"] = sfObject["Does_the_employer_have_50_or_more_employ__c"].ToString();
                }
                if ((item["Arethereaffiliatedsubsidiariesoraffiliatedentitiestobeincludedunderthe"] == null ? false : sfObject["Are_there_affiliated_subsidiaries_or_aff__c"] != null))
                {
                    item["Arethereaffiliatedsubsidiariesoraffiliatedentitiestobeincludedunderthe"] = sfObject["Are_there_affiliated_subsidiaries_or_aff__c"].ToString();
                }
                if ((item["BRCAdditionalInformation"] == null ? false : sfObject["BRC_Additional_Information__c"] != null))
                {
                    item["BRCAdditionalInformation"] = sfObject["BRC_Additional_Information__c"].ToString();
                }
                if ((item["SalesForcePlanID"] == null ? false : sfObject["Id"] != null))
                {
                    item["SalesForcePlanID"] = sfObject["Id"].ToString();
                }
            }
        }
    }
}
