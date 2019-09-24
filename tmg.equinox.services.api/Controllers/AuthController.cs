using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.ConsumerAccount;
using tmg.equinox.services.api.Framework;
using tmg.equinox.services.api.Models;
using tmg.equinox.services.api.Validators;
using Microsoft.Owin;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using tmg.equinox.applicationservices.viewmodels.Settings;

namespace tmg.equinox.services.api.Controllers
{
    [Authorize]
    public class AuthController : BaseApiController
    {
        private IUserManagementService _userManagementService;
       
        public AuthController(IUserManagementService userManagementService)
        {
            _userManagementService = userManagementService;
        }


        /*[Route("api/v1/Auth/user/{name}")]
        public HttpResponseMessage GetUser(string userName)
        {
            //Only SuperAdmin or Admin can delete users (Later when implement roles)
            var user = this.AppUserManager.FindByName(userName);

            if (user != null)
            {
                return CreateResponse(new { Status = Validators.Constants.Success, Result = this.TheModelFactory.Create(user) });
            }
            return CreateResponse(new { Status = Validators.Constants.Failure, Message = Validators.Constants.NotExist });        
        }*/
        /// <summary>
        /// Get the list of application users
        /// </summary>
        /// <returns></returns>
        [Route("api/v1/Auth/users")]
        [HttpGet]
        [ResponseType(typeof(List<UserManagementSettingsViewModel>))]

        public HttpResponseMessage GetUsers()
        {
            //Only SuperAdmin or Admin can delete users (Later when implement roles)
            var user = _userManagementService.GetUsers();

            if (user != null)
            {
                return CreateResponse(new { Status = Validators.Constants.Success, Result = user });
            }
            return CreateResponse(new { Status = Validators.Constants.Failure, Message = Validators.Constants.NotExist });
        }
        /// <summary>
        /// Get the user detail
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [Route("api/v1/Auth/user/{name}")]
        [HttpGet]
        [ResponseType(typeof(UserManagementSettingsViewModel))]

        public HttpResponseMessage GetUseDetail(string name)
        {
            //Only SuperAdmin or Admin can delete users (Later when implement roles)
            var user = _userManagementService.GetUserDetails(1, name);

            if (user != null && user.Count > 0)
            {
                return CreateResponse(new { Status = Validators.Constants.Success, Result = user });
            }
            return CreateResponse(new { Status = Validators.Constants.Failure, Message = Validators.Constants.NotExist });
        }
        /// <summary>
        /// Get the Role Name by Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [Route("api/v1/Auth/Role/{id}")]
        [HttpGet]
        [ResponseType(typeof(List<RoleReturnModel>))]

        public HttpResponseMessage GetRole(int Id)
        {
            var role = this.AppRoleManager.FindByIdAsync(Id);

            if (role.Result != null)
            {            
                return CreateResponse(new { Status = Validators.Constants.Success, Result = this.TheModelFactory.Create(role.Result) });
            }
            return CreateResponse(new { Status = Validators.Constants.Failure, Message = Validators.Constants.NotExist });
        }
        /// <summary>
        /// Update associated role with new role
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userRole"></param>
        /// <param name="newUserRole"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPut]
        [ResponseType(typeof(List<ReponseMessage>))]
        [Route("api/v1/Auth/UserRole")]
        public HttpResponseMessage UpdateRole(string userName, string userRole, string newUserRole, int userId)
        {

            ServiceResult result = this._userManagementService.UpdateRole(userName, newUserRole, userId, CurrentUserName);
            if (result.Result == ServiceResultStatus.Success)
            {
                return CreateResponse(new ReponseMessage { Status = Validators.Constants.Success, Message = Validators.Constants.UpdateSuccess });
            }
            else
            {
                return CreateResponse(new ReponseMessage  { Status = Validators.Constants.Failure, Message = Validators.Constants.UpdateFail });
            }
        }
        /// <summary>
        /// Create Application User, Password will be set default
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="role"></param>
        /// <param name="email"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(List<ReponseMessage>))]
        [Route("api/v1/Auth/User")]
        public HttpResponseMessage Create(string userName, string role, string email, string firstName, string lastName)
        {

            var user = this.TheModelFactory.Create(userName, role, email, firstName, lastName);

            try
            {
            

                var result = this.AppUserManager.Create(user);
                if (result.Succeeded)
                {
                    AppUserManager.AddToRole(user.Id, role);

                    return CreateResponse(new ReponseMessage { Status = Validators.Constants.Success, Message = Validators.Constants.CreateSuccess });
                }
                else
                {
                    return CreateResponse(new { Status = Validators.Constants.Failure, Message = result.Errors });
                }
            }
            catch(Exception e)
            {
                return CreateResponse(new { Status = Validators.Constants.Failure, Message = Validators.Constants.UpdateFail });
            }

            
        }
        /// <summary>
        /// Get List of role Name
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(List<RoleViewModel>))]
        [Route("api/v1/Auth/Roles")]
        public HttpResponseMessage GetAllRoles()
        {
            var roles = _userManagementService.GetRoles();

            if (roles != null)
            {
                return CreateResponse(new { Status = Validators.Constants.Success, Result = roles });
            }
            return CreateResponse(new { Status = Validators.Constants.Failure, Message = Validators.Constants.NotExist });
        }

            /*   [Route("api/v1/Auth/Roles")]
               public HttpResponseMessage GetAllRoles()
               {
                   var roles = this.AppRoleManager.Roles.ToList();

                   if (roles != null)
                   {
                       return CreateResponse(new { Status = Validators.Constants.Success, Result = this.TheModelFactory.Create(roles) });
                   }
                   return CreateResponse(new { Status = Validators.Constants.Failure, Message = Validators.Constants.NotExist });
                 //  return Ok(roles);
               }*/

            /*
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
                if (result.Items != null && result.Items.Count()>0)
                {
                    if (result.Result == ServiceResultStatus.Success)
                        return CreateResponse(new { Status = "Success", Message = result.Items.First().Messages.First() });
                    else if (result.Result == ServiceResultStatus.Failure)
                        return CreateResponse(new { Status = "Failure", Message = result.Items.First().Messages.First() });
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
                if (result.Items != null && result.Items.Count() > 0)
                {
                    if (result.Result == ServiceResultStatus.Success)
                        return CreateResponse(new { Status = "Success", Message = result.Items.First().Messages.First() });
                    else if (result.Result == ServiceResultStatus.Failure)
                        return CreateResponse(new { Status = "Failure", Message = result.Items.First().Messages.First() });
                }
                return CreateResponse(result);
            }*/

        }
    }

