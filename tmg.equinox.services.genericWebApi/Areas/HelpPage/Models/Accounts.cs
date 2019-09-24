using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tmg.equinox.applicationservices;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.dependencyresolution;
using tmg.equinox.infrastructure.exceptionhandling;

namespace tmg.equinox.services.genericWebApi.Areas.Help.Model
{
    public class Accounts
    {
        private IConsumerAccountService _serviceAccount { get; set; }
        private int tenantId = 1;
        public Accounts()
        {
            _serviceAccount = UnityConfig.Resolve<IConsumerAccountService>();
        }
        public int ID { get; set; }
        public string Name { get; set; }
        public IList<urls> url { get; set; }

        public List<Accounts> GetAccoountList()
        {
            List<Accounts> lstAccount = null;
            try
            {
                lstAccount = (from ac in this._serviceAccount.GetAccountList(tenantId)
                              select new Accounts
                              {
                                  ID = Convert.ToInt32(ac.AccountID),
                                  Name = ac.AccountName,
                                  url = (new[] { new urls { account = "/api/data/v1.0/Accounts", rowTemplate = "/api/data/v1.0/Accounts/Account/" + ac.AccountID } }),
                              }).ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                return null;
            }
            return lstAccount;
        }
        public List<Accounts> GetAccoountList(string accountID)
        {
            List<Accounts> lstAccount = null;
            try
            {
                lstAccount = (from ac in this._serviceAccount.GetAccountList(tenantId)
                              where ac.AccountID == int.Parse(accountID)
                              select new Accounts
                              {
                                  ID = Convert.ToInt32(ac.AccountID),
                                  Name = ac.AccountName,
                                  url = (new[] { new urls { account = "/api/data/v1.0/Accounts", rowTemplate = "/api/data/v1.0/Accounts/Account/" + ac.AccountID } }),
                              }).ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                return null;
            }
            return lstAccount;
        }

        public DelResponse DeleteAccountList(string accountID, string updatedBy)
        {
            try
            {
                var accId = Convert.ToInt32(accountID);
                ServiceResult result = this._serviceAccount.DeleteAccount(tenantId, accId, updatedBy);

                if (result.Result == ServiceResultStatus.Success)
                {
                    DelResponse Del1 = new DelResponse() { id = accountID, errors = "[]", success = "true" };
                    return Del1;
                }
                else
                {
                    DelResponse Del2 = new DelResponse() { id = accountID, errors = result.ToString(), success = "false" };
                    return Del2;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                return null;
            }
        }
    }
    public class DelResponse
    {
        public string id { get; set; }
        public string errors { get; set; }
        public string success { get; set; }
    }
    public class urls
    {
        public string account { get; set; }
        public string rowTemplate { get; set; }
    }
    public class Account
    {
        private IConsumerAccountService _serviceAccount { get; set; }
        private int tenantId = 1;

        public Account()
        {
            _serviceAccount = UnityConfig.Resolve<IConsumerAccountService>();
        }
        public string ID { get; set; }
        public bool success { get; set; }
        public string Errors { get; set; }

        public ServiceResult AddAccount(string name, string addedBy)
        {
            ServiceResult result = null;
            try
            {
                result = this._serviceAccount.AddAccount(tenantId, name, addedBy);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
            }
            return result;
        }
    }
}   