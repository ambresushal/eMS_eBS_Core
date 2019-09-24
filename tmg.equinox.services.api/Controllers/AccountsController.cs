using Newtonsoft.Json.Linq;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Description;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.ConsumerAccount;
using tmg.equinox.services.api.Framework;
using tmg.equinox.services.api.Models;
using tmg.equinox.services.api.Validators;

namespace tmg.equinox.services.api.Controllers
{
    [Authorize]
    public class AccountsController: BaseApiController
    {
        private IConsumerAccountService _accountService;
       
        public AccountsController(IConsumerAccountService consumerAccountService)
        {
            _accountService = consumerAccountService;
        }

        /// <summary>
        /// Get the list of the accounts.
        /// </summary>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/Accounts")]
        [ResponseType(typeof(AccountViewModel))]
        public HttpResponseMessage Get(int pageNo = 1, int pageSize = 10)
        {
            int skip = (pageNo - 1) * pageSize;
            int total = 0;

            var account = _accountService.GetAccountList(skip, pageSize, ref total);

            var linkBuilder = new PageLinkBuilder(Url, "", null, pageNo, pageSize, total);

            //var response = Request.CreateResponse(HttpStatusCode.OK, new PagedResult<AccountViewModel>(account, pageNo, pageSize, total));
            var response = CreateResponse(new { Status = "Success", Result = new PagedResult<AccountViewModel>(account, pageNo, pageSize, total) });

            response.Headers.Add("Link", string.Join(", ", PageLinkBuilder.GetPageLink(linkBuilder)));

            return response;

        }

        /// <summary>
        /// Get an Account by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/Accounts/{id}")]
        [ResponseType(typeof(AccountViewModel))]
        public HttpResponseMessage GetAccountById(int id)
        {
            var account = _accountService.GetAccount(id);
            if (account == null)
            {
                return CreateResponse(new { Status = "Failure", Message = Constants.AccountNotExist });
            }
            return CreateResponse(new { Status = "Success", Result = account });
        }

        ///<summary>
        ///Get Account by Name.
        ///</summary>
        ///<param name="account"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v1/Accounts/AccountName")]
        [ResponseType(typeof(AccountViewModel))]
        public HttpResponseMessage GetAccountByName([FromBody]GetAccount account)
        {
            var accountData = _accountService.GetAccountByName(account.AccountName);
            if (accountData == null)
            {
                return CreateResponse(new { Status = "Failure", Message = Constants.AccountNotExist });
            }
            return CreateResponse(new { Status = "Success", Result = accountData });
        }

        /// <summary>
        /// Add a new Account.
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v1/Accounts")]
        public HttpResponseMessage Add([FromBody]GetAccount account)
        {
            string resultMessage = string.Empty;
            ServiceResult result = _accountService.AddAccount(base.TenantId, account.AccountName,CurrentUserName);
            if (result.Result == ServiceResultStatus.Success)
                return CreateResponse(new { Status = "Success", Message = "Account added Successfully.", AccountID = result.Items.First().Messages.First() });
             else
            {
                if (result.Result == ServiceResultStatus.Failure)
                {
                    if (result.Items != null && result.Items.Count() != 0)
                        return CreateResponse(new { Status = "Failure", Message = "Account Name already exist.", AccountID = result.Items.First().Messages.First() });
                }
            }
            return CreateResponse(result);
        }

        /// <summary>
        /// Update an existing Account by id.
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/v1/Accounts")]
        public HttpResponseMessage Update([FromBody] Account account)
        {
            string resultMessage = string.Empty;
            ServiceResult result = _accountService.UpdateAccount(base.TenantId, account.AccountID, account.AccountName, CurrentUserName);
            if (result.Result == ServiceResultStatus.Success || result.Result == ServiceResultStatus.Failure || result.Items != null && result.Items.Count()>0)
            {
                if (result.Result == ServiceResultStatus.Success)
                    return CreateResponse(new { Status = "Success", Message = "Updated Successfully!" });
                else if (result.Result == ServiceResultStatus.Failure)
                    return CreateResponse(new { Status = "Failure", Message = "Failed to Update!" });
            }
            return CreateResponse(result);
        }

        /// <summary>
        /// Delete an Account by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/v1/Accounts/{id}")]
        public HttpResponseMessage Delete(int id)
        {
            string resultMessage = string.Empty;
            ServiceResult result = _accountService.DeleteAccount(base.TenantId, id, CurrentUserName);
            if (result.Result == ServiceResultStatus.Success || result.Result == ServiceResultStatus.Failure || result.Items != null && result.Items.Count() > 0)
            {
                if (result.Result == ServiceResultStatus.Success)
                    return CreateResponse(new { Status = "Success", Message = "Deleted Successfully!" });
                else if (result.Result == ServiceResultStatus.Failure)
                    return CreateResponse(new { Status = "Failure", Message = "Failed to delete!" });
            }
            return CreateResponse(result);
        }

    }
}
