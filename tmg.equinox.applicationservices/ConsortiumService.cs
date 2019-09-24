using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.Consortium;
using tmg.equinox.repository.interfaces;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.infrastructure.exceptionhandling;
using System.Diagnostics.Contracts;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.repository.extensions;

namespace tmg.equinox.applicationservices
{
    public class ConsortiumService : IConsortiumService
    {

        private IUnitOfWorkAsync _unitOfWork { get; set; }
        private ILoggingService _loggingService { get; set; }

        public ConsortiumService(IUnitOfWorkAsync unitOfWork, ILoggingService loggingService)
        {
            this._unitOfWork = unitOfWork;
            this._loggingService = loggingService;
        }

        /// <summary>
        /// Gets the list of Consortium.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <returns></returns>
        public GridPagingResponse<ConsortiumViewModel> GetConsortiumList(int tenantID, GridPagingRequest gridPagingRequest)
        {
            List<ConsortiumViewModel> consortiumList = null;
            int count = 0;

            try
            {
                SearchCriteria criteria = new SearchCriteria();
                criteria = JqGridHelper.GetCriteria(gridPagingRequest.filters);

                consortiumList = (from con in this._unitOfWork.RepositoryAsync<Consortium>()
                                       .Query()
                                       .Filter(con => con.TenantID == tenantID && con.IsActive == true)
                                       .Get()
                                  select new ConsortiumViewModel
                                 {
                                     ConsortiumID = con.ConsortiumID,
                                     ConsortiumName = con.ConsortiumName,
                                     IsActive = con.IsActive
                                 })
                                 .ApplySearchCriteria(criteria)
                                 .ApplyOrderBy(gridPagingRequest.sidx, gridPagingRequest.sord).ToList()
                                   .GetPage(gridPagingRequest.page, gridPagingRequest.rows, out count);

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            //return consortiumList;
            return new GridPagingResponse<ConsortiumViewModel>(gridPagingRequest.page, count, gridPagingRequest.rows, consortiumList);
        }

        /// <summary>
        /// Adds the Consortium.
        /// </summary>
        /// <param name="tenantId">tenant identifier.</param>
        /// <param name="accountName">Name of the Consortium.</param>
        /// <param name="addedBy">added by.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Consortium Name already exists</exception>
        public ServiceResult AddConsortium(string consortiumName, int tenantID, string addedBy)
        {
            ServiceResult result = new ServiceResult();
            int consortiumID = 0;
            try
            {
                if (!this._unitOfWork.RepositoryAsync<Consortium>().IsConsortiumNameExists(tenantID, consortiumID, consortiumName))
                {
                    Consortium consortiumToAdd = new Consortium();
                    consortiumToAdd.ConsortiumName = consortiumName;
                    consortiumToAdd.TenantID = tenantID;
                    consortiumToAdd.AddedBy = addedBy;
                    consortiumToAdd.AddedDate = DateTime.Now;
                    consortiumToAdd.UpdatedBy = null;
                    consortiumToAdd.UpdatedDate = null;
                    consortiumToAdd.IsActive = true;

                    this._unitOfWork.RepositoryAsync<Consortium>().Insert(consortiumToAdd);
                    this._unitOfWork.Save();

                    result.Result = ServiceResultStatus.Success;
                }
                else
                {
                    result.Result = ServiceResultStatus.Failure;
                }
            }
            catch (Exception ex)
            {
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return result;
        }

        /// <summary>
        /// Updates the Consortium.
        /// </summary>
        /// <param name="tenantId">tenant identifier.</param>
        /// <param name="accountId">consortium identifier.</param>
        /// <param name="accountName">Name of the consortium.</param>
        /// <param name="updatedBy">updated by.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">
        /// Consortium Name Alraedy exists
        /// or
        /// Consortium Does Not exists
        /// </exception>
        public ServiceResult UpdateConsortium(int consortiumID, string consortiumName, int tenantID, string updatedBy)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                if (!this._unitOfWork.RepositoryAsync<Consortium>().IsConsortiumNameExists(tenantID, consortiumID, consortiumName))
                {
                    Consortium consortiumToUpdate = this._unitOfWork.RepositoryAsync<Consortium>()
                                                           .Query()
                                                           .Filter(c => c.ConsortiumID == consortiumID)
                                                           .Get().FirstOrDefault();

                    consortiumToUpdate.ConsortiumName = consortiumName;
                    consortiumToUpdate.UpdatedBy = updatedBy;
                    consortiumToUpdate.UpdatedDate = DateTime.Now;

                    this._unitOfWork.RepositoryAsync<Consortium>().Update(consortiumToUpdate);
                    this._unitOfWork.Save();

                    result.Result = ServiceResultStatus.Success;
                }
                else
                {
                    result.Result = ServiceResultStatus.Failure;
                }
            }
            catch (Exception ex)
            {
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return result;

        }

        /// <summary>
        /// Gets the list of Consortium.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <returns></returns>
        public IEnumerable<ConsortiumViewModel> GetConsortiumForDropdown(int tenantID)
        {
            IEnumerable<ConsortiumViewModel> consortiumList = null;

            try
            {
                consortiumList = (from con in this._unitOfWork.RepositoryAsync<Consortium>()
                                       .Query()
                                       .Filter(con => con.TenantID == tenantID && con.IsActive ==  true)
                                       .Get()
                                  select new ConsortiumViewModel
                                 {
                                     ConsortiumID = con.ConsortiumID,
                                     ConsortiumName = con.ConsortiumName,
                                     IsActive = con.IsActive
                                 });

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return consortiumList;            
        }

        /// <summary>
        /// Gets details of a Consortium.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <returns></returns>
        public ConsortiumViewModel GetConsortium(int folderVersionID)
        {
            ConsortiumViewModel consortium = null;
             try
            {
                consortium = (from fldv in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                                     join con in this._unitOfWork.RepositoryAsync<Consortium>().Get()
                                     on fldv.ConsortiumID equals con.ConsortiumID
                                     where (con.IsActive ==true && fldv.FolderVersionID == folderVersionID)
                                  select new ConsortiumViewModel
                                 {
                                     ConsortiumID = con.ConsortiumID,
                                     ConsortiumName = con.ConsortiumName,                                     
                                 }).FirstOrDefault();

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }

             return consortium;
        }

        public int? GetConsortiumId(string consortiumName)
        {
            int? consortiumID = 0;
            consortiumID = this._unitOfWork.RepositoryAsync<Consortium>().Get().Where(s => s.ConsortiumName == consortiumName).Select(s => s.ConsortiumID).FirstOrDefault();
            return consortiumID;
        }
    }
}
