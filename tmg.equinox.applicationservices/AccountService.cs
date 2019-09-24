using System;
using System.Linq;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.Account;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.applicationservices
{
    public class AccountService : IAccountService
    {
        #region Private Memebers

        private IUnitOfWorkAsync _unitOfWork { get; set; }
        private ILoggingService _loggingService { get; set; }
        private UserActivityProfile _userActivityProfile { get; set; }

        #endregion Private Members

        #region Constructor

        public AccountService(IUnitOfWorkAsync unitOfWork, ILoggingService loggingService)
        {
            this._unitOfWork = unitOfWork;
            this._loggingService = loggingService;
           
        }

        #endregion Constructor

        public LoginViewModel FindUser(int tenantId, string username, string password)
        {
            LoginViewModel userDetailsList = null;
            try
            {
                userDetailsList = (from us in this._unitOfWork.RepositoryAsync<User>()
                                       .Query()
                                       .Filter(c => c.UserName == username && c.PasswordHash == password)
                                       .Get()
                                   select new LoginViewModel()
                                   {
                                       UserId = us.UserID,
                                       UserName = us.UserName,
                                       Password = us.PasswordHash
                                   }).FirstOrDefault();

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return userDetailsList;
        }

        public LoginViewModel FindUser(int tenantId, string username)
        {
            LoginViewModel userDetailsList = null;
            try
            {
                userDetailsList = (from us in this._unitOfWork.RepositoryAsync<User>()
                                       .Query()
                                       .Filter(c => c.UserName == username )
                                       .Get()
                                   select new LoginViewModel()
                                   {
                                       UserId = us.UserID,
                                       UserName = us.UserName,
                                       Password = us.PasswordHash
                                   }).FirstOrDefault();

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return userDetailsList;
        }        
    }
}
