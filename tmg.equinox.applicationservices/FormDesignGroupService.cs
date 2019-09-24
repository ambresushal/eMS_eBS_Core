using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FormDesign;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;
using System.Diagnostics.Contracts;
using System.Transactions;
using System.Text.RegularExpressions;
using tmg.equinox.repository.extensions;
using tmg.equinox.applicationservices.viewmodels.FormDesignGroup;
using tmg.equinox.infrastructure.exceptionhandling;

namespace tmg.equinox.applicationservices
{
    public class FormDesignGroupService : IFormDesignGroupService
    {
        #region Private Memebers
        private IUnitOfWorkAsync _unitOfWork { get; set; }
        #endregion Private Members

        #region Public Properties

        #endregion Public Properties

        #region Constructor
        public FormDesignGroupService(IUnitOfWorkAsync unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        #endregion Constructor

        #region Public Methods
        public IEnumerable<FormDesignGroupRowModel> GetFormDesignGroupList(int tenantId)
        {
            Contract.Requires(tenantId > 0, "Invalid tenantId");
            IList<FormDesignGroupRowModel> formDesignGroupList = null;
            try
            {

                formDesignGroupList = (from c in this._unitOfWork.RepositoryAsync<FormDesignGroup>()
                                                                        .Query()
                                                                        .Filter(c => c.TenantID == tenantId && c.IsMasterList==false)
                                                                        .Get()
                                  select new FormDesignGroupRowModel
                                  {
                                       FormGroupId = c.FormDesignGroupID,
                                       FormDesignGroupName = c.GroupName,
                                       TenantID = c.TenantID ,                                 
                                       AddedBy = c.AddedBy,
                                       AddedDate = c.AddedDate,
                                       UpdatedBy = c.UpdatedBy,
                                       UpdatedDate = c.UpdatedDate
                                  }).ToList();

                if (formDesignGroupList.Count() == 0)
                    formDesignGroupList = null;

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return formDesignGroupList;
        }

        private int GetSequence(int ? val , int cnt)
        {
            int returnval;
            if (val.HasValue)
               returnval= val.Value;
            else
                returnval=cnt++;
            return returnval;
        }
       /// <summary>
        /// Handled Null (formDesignMappedList) object
        /// by Snehal K on 20140901
       /// </summary>
       /// <param name="tenantId"></param>
       /// <param name="formGroupId"></param>
       /// <returns></returns>
        public IEnumerable<FormGroupFormRowModel> GetFormDesignList(int tenantId, int formGroupId)
        {
            Contract.Requires(tenantId > 0, "Invalid tenantId");
            int seq = 1;
            IList<FormGroupFormRowModel> formDesignMappedList = null;
            try
            {           
                //formDesignMappedList = (from c in this._unitOfWork.Repository<FormDesign>()
                //                                                        .Query()
                //                                                        .Filter(c => c.TenantID == tenantId)                                                                        
                //                                                        .Get()                                                                        
                //                       select new FormGroupFormRowModel
                //                       {
                //                           FormDesignId = c.FormID,
                //                           FormDesignName = c.FormName,                                                                                    
                //                           TenantID = c.TenantID , 
                //                           Abbreviation = c.Abbreviation,
                //                           IsIncluded = c.FormDesignGroupMappings.Where(p=>p.FormDesignGroupID==formGroupId).FirstOrDefault() !=null? true:false,
                //                           AllowMultipleInstance = c.FormDesignGroupMappings.Where(p=>p.FormDesignGroupID==formGroupId).FirstOrDefault().AllowMultipleInstance.HasValue?
                //                           c.FormDesignGroupMappings.Where(p=>p.FormDesignGroupID==formGroupId).FirstOrDefault().AllowMultipleInstance.Value:false,
                //                           Sequence = c.FormDesignGroupMappings.Where(p => p.FormDesignGroupID == formGroupId).FirstOrDefault().Sequence.HasValue ? 
                //                           c.FormDesignGroupMappings.Where(p => p.FormDesignGroupID == formGroupId).FirstOrDefault().Sequence.Value: 0                                                                          
                //                       }).ToList();

                //Above code is commented because system should allow a form design to be inculded only into one group. 
                formDesignMappedList = (from frm in this._unitOfWork.RepositoryAsync<FormDesign>().Get()
                                        join fdgm in this._unitOfWork.RepositoryAsync<FormDesignGroupMapping>().Get()
                                        on frm.FormID equals fdgm.FormID into fdgmgroupjoin
                                        from fldvrsnsubList in fdgmgroupjoin.DefaultIfEmpty()
                                        where (fldvrsnsubList.FormDesignGroupID == formGroupId || fldvrsnsubList.FormDesignGroupID == null) && frm.TenantID == tenantId && frm.IsActive == true
                                        select new FormGroupFormRowModel
                                        {
                                            FormDesignId = frm.FormID,
                                            FormDesignName = frm.FormName,
                                            TenantID = frm.TenantID,
                                            Abbreviation = frm.Abbreviation,
                                            IsIncluded = fldvrsnsubList.FormDesignGroupID != null ? true : false,
                                            AllowMultipleInstance = fldvrsnsubList.AllowMultipleInstance.HasValue ? fldvrsnsubList.AllowMultipleInstance.Value : false,
                                            Sequence = fldvrsnsubList.Sequence.HasValue ? fldvrsnsubList.Sequence.Value : 0
                                        }).ToList();

                if (formDesignMappedList != null && formDesignMappedList.Any())
                    formDesignMappedList.OrderBy(c => c.Sequence);
             
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return formDesignMappedList;
        }

        public ServiceResult AddFormGroup(string userName, int tenantId, string groupName, bool isMasterList)
        {
            Contract.Requires(!string.IsNullOrEmpty(userName), "Username cannot be empty");
            Contract.Requires(tenantId > 0, "Invalid tenantId");
            Contract.Requires(!string.IsNullOrEmpty(groupName), "FormGroupname cannot be empty");
            ServiceResult result = new ServiceResult();
            try
            {
                FormDesignGroup itemToAdd = new FormDesignGroup();
                itemToAdd.TenantID = tenantId;
                itemToAdd.GroupName = groupName;
                itemToAdd.AddedBy = userName;
                itemToAdd.AddedDate = DateTime.Now;
                itemToAdd.IsMasterList = isMasterList;
                //Call to repository method to insert record.
                this._unitOfWork.RepositoryAsync<FormDesignGroup>().Insert(itemToAdd);
                this._unitOfWork.Save();              

                //Return success result
                result.Result = ServiceResultStatus.Success;
                List<ServiceResultItem> items = new List<ServiceResultItem>();
                items.Add(new ServiceResultItem { Messages = new string[] { itemToAdd.FormDesignGroupID.ToString()}});
                result.Items = items;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                //Get all the exception/inner exception messages and set the return code to failure
                result = ex.ExceptionMessages();
            }
            return result;
        }

        public ServiceResult UpdateFormGroup(string userName, int tenantId, int formGroupId,string groupName)
        {
            Contract.Requires(!string.IsNullOrEmpty(userName), "Username cannot be empty");
            Contract.Requires(tenantId > 0, "Invalid tenantId");
            Contract.Requires(formGroupId > 0, "Invalid formGroupId");
            Contract.Requires(!string.IsNullOrEmpty(groupName), "GroupName cannot be empty");
            ServiceResult result = new ServiceResult();
            try
            {
                FormDesignGroup itemToUpdate = this._unitOfWork.RepositoryAsync<FormDesignGroup>()
                                                               .FindById(formGroupId);

                if (itemToUpdate != null)
                {
                    itemToUpdate.GroupName = groupName;                 
                    itemToUpdate.UpdatedBy = userName;
                    itemToUpdate.UpdatedDate = DateTime.Now;

                    //Call to repository method to Update record.
                    this._unitOfWork.RepositoryAsync<FormDesignGroup>().Update(itemToUpdate);
                    this._unitOfWork.Save();

                    //Return success result
                    result.Result = ServiceResultStatus.Success;
                }
                else
                {
                    result.Result = ServiceResultStatus.Failure;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                result = ex.ExceptionMessages();
            }
            return result;
        }

        /// <summary>
        /// Used DeleteAsync instead of Delete 
        /// by SK on 9/15/2014
        /// Need to make Delete method async if the call is long running
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="tenantId"></param>
        /// <param name="formGroupId"></param>
        /// <param name="formDesignsToMap"></param>
        /// <returns></returns>
        public async Task<ServiceResult> UpdateFormGroupMapping(string userName, int tenantId, int formGroupId, IList<FormGroupFormRowModel>formDesignsToMap)
        {
            Contract.Requires(!string.IsNullOrEmpty(userName), "Username cannot be empty");
            Contract.Requires(tenantId > 0, "Invalid tenantId");
            Contract.Requires(formGroupId > 0, "Invalid formGroupId");          
            ServiceResult result = new ServiceResult();
            try
            {
                //Delete  all the formdesign associations  this formgroup
                var formDesignMappings = (from c in this._unitOfWork.RepositoryAsync<FormDesignGroupMapping>()
                                                                        .Query()
                                                                        .Filter(c => c.FormDesignGroup.TenantID == tenantId && c.FormDesignGroupID == formGroupId)
                                                                        .Get()
                                                                        select c.FormDesignGroupMappingID).ToList();

                if (formDesignMappings != null && formDesignMappings.Count() > 0)
                {
                    foreach (var formDesignMapping in formDesignMappings)
                    {
                       await this._unitOfWork.RepositoryAsync<FormDesignGroupMapping>().DeleteAsync(formDesignMapping);
                    }
                }

                //Insert all or none  formdesigngroup mappings               

                if (formDesignsToMap != null && formDesignsToMap.Count > 0)
                {
                    foreach (FormGroupFormRowModel formdesignRowModel in formDesignsToMap)
                    {
                        if (formdesignRowModel.IsIncluded)
                        {
                            FormDesignGroupMapping fromDesignToMap = new FormDesignGroupMapping
                            {
                                FormID = formdesignRowModel.FormDesignId,
                                FormDesignGroupID = formGroupId,
                                Sequence = formdesignRowModel.Sequence,
                                AccessibleToRoles = "All",     //TO DO : While implementing the Roles
                                AllowMultipleInstance = formdesignRowModel.AllowMultipleInstance
                            };
                            this._unitOfWork.RepositoryAsync<FormDesignGroupMapping>().Insert(fromDesignToMap);
                        }
                    }
                }                  
               using (var scope = new TransactionScope())
               {
                  this._unitOfWork.Save();
                  scope.Complete();
               }
               result.Result = ServiceResultStatus.Success;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                result = ex.ExceptionMessages();
            }
            return result;
        
        }
        #endregion Public Methods

        #region Private Methods
      
        #endregion Private Methods
    }
}
