using System;
using Owin;
using System.Web.Http;
using System.Net;
using Microsoft.Owin.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Collections.Generic;
using Microsoft.Owin.Hosting;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;
using Newtonsoft.Json;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.ConsumerAccount;
using tmg.equinox.services.api.Models;
using tmg.equinox.services.api.Validators;

namespace tmg.equinox.services.api.test
{
    [TestClass]
    public class AccountTest : BaseTest
    {
        public override void Initialise()
        {
           
        }

        [TestMethod]
        public  void Test_Account_Get()
        {
            var response = GetPageResult<AccountViewModel>("api/v1/Accounts");
            Assert.IsNotNull(response);
        }
        [TestMethod]
        public void Test_Account_GetByID()
        {
            var response = Get<AccountViewModel>("api/v1/Accounts/124");
            Assert.IsNotNull(response.Result);
            Assert.AreEqual(response.Status, Const.Success);
            Assert.AreEqual(response.Result.AccountID, 124);
        }

        [TestMethod]
        public void Test_Account_GetByID_Not_Found()
        {
            var response = Get<AccountViewModel>("api/v1/Accounts/1");
            Assert.IsNull(response.Result);
            Assert.AreEqual(response.Message, Constants.AccountNotExist);
            Assert.AreEqual(response.Status, Const.Failure);
        }

        [TestMethod]
        public void Test_Account_New_Account()
        {
           
            var response = Post<AccountViewModel>("api/v1/Accounts", new GetAccount {  AccountName ="Nitin_test"});

            if (response.Status == Const.Success)
            {
                Assert.IsNull(response.Result);
                Assert.AreEqual(response.Message, "Account added Successfully.");
                Assert.AreEqual(response.Status, Const.Success);
            }
            else
            {
                Assert.AreEqual(response.Message, "Account Name already exist.");
            }
        }

        [TestMethod]
        public void Test_Account_New_Update_Delete()
        {
            //create new
            Test_Account_New_Account();

            //get 
            var response = Post<AccountViewModel>("api/v1/Accounts/AccountName", new GetAccount { AccountName = "Nitin_test" });
            
            var account = new Account
            {
                AccountID = response.Result.AccountID
                ,AccountName = "Nitin T"
                , IsActive = response.Result.IsActive
            };
            //do update
            var update = Put<AccountViewModel>("api/v1/Accounts", account);

            //delete it
            var delete = Delete<AccountViewModel>("api/v1/Accounts/" + account.AccountID);

            Assert.IsNull(delete.Result);
            Assert.AreEqual(delete.Message, "Account Deleted Successfully.");
            Assert.AreEqual(delete.Status, Const.Success);
            
        }
    }
}
