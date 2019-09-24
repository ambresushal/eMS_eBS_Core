using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.services.genericWebApi.Areas.Help.Model;

namespace tmg.equinox.services.genericWebApi.Controllers
{
    [Authorize]
    public partial class AccountsController : ApiController
    {
        // GET api/<controller>
        /// <summary>
        /// Gets all accounts data from the eBenefitSync.
        /// </summary>
        [HttpGet]
        public IHttpActionResult Get()
        {
            Accounts acc = new Accounts();
            IList<Accounts> accountList = acc.GetAccoountList();
            if (accountList == null || accountList.Count == 0)
                return BadRequest("Account Not Found!");
            else
                return Ok(accountList); 
        }
        // GET api/<controller>/5
        /// <summary>
        /// Gets specific account data from the eBenefitSync.
        /// </summary>
        [HttpGet]
        [ActionName("Account")]
        public IHttpActionResult Get(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("Please enter Account Id!");
            Accounts acc = new Accounts();
            IList<Accounts> accountList;
            accountList = acc.GetAccoountList(id);
            if (accountList == null || accountList.Count == 0)
                return BadRequest("Account Not Found!");
            else
                return Ok(accountList);
        }
        /// <summary>
        /// Add a new account in eBenefitSync.
        /// </summary>
        [HttpPost]
        [ActionName("AddAccount")]
        public IList<Account> PostAddAccount(string AccountName, string AddedBy)
        {
            if(string.IsNullOrEmpty(AccountName)  || string.IsNullOrEmpty(AddedBy))
                return new[] { new Account { ID = "", Errors = "Please enter Account Name and Added By!", success = false } };
            Account addAccount = new Account();
            ServiceResult isAccountAdded = addAccount.AddAccount(AccountName, AddedBy);
            string accountID = string.Empty;
            bool result = false;
            string errorMessage = "Error While Account Create!";
            if (isAccountAdded.Items.Count() > 0)
            {
                accountID = isAccountAdded.Items.FirstOrDefault().Messages[0];
                result = true;
                errorMessage = string.Empty;
            }
            return new[] { new Account { ID = accountID, Errors = errorMessage, success = result } };
        }
        // PUT api/<controller>/5
        //To delete account
        /// <summary>
        /// Delete specific accounts from the eBenefitSync.
        /// </summary>
        [HttpPut]
        [ActionName("DeleteAccount")]
        public IHttpActionResult PuttoDeleteAccount(string id, string updatedby)
        {
            if(string.IsNullOrEmpty(id) || string.IsNullOrEmpty(updatedby))
                return BadRequest("Please enter ID and UpdatedBy!");
            Accounts al = new Accounts();
            DelResponse accountList;
            accountList = al.DeleteAccountList(id, updatedby);
            if (accountList == null)
                return BadRequest("Error While Account Delete!");
            else
                return Ok(accountList);
        }
    }
}