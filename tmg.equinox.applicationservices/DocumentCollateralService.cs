using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.DocumentCollateral;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.applicationservices
{
    public class DocumentCollateralService : IDocumentCollateralService
    {
        private IUnitOfWorkAsync _unitOfWork { get; set; }

        public DocumentCollateralService(IUnitOfWorkAsync unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public IEnumerable<DocumentCollateralViewModel> GetDocumentList(int tenantID)
        {
            List<DocumentCollateralViewModel> documentList = null;
            try
            {

                documentList = (from fi in this._unitOfWork.RepositoryAsync<FormInstance>().Get()
                                join fldver in this._unitOfWork.RepositoryAsync<FolderVersion>().Get() on fi.FolderVersionID equals fldver.FolderVersionID
                                join fld in this._unitOfWork.RepositoryAsync<Folder>().Get() on fldver.FolderID equals fld.FolderID
                                join afm in this._unitOfWork.RepositoryAsync<AccountFolderMap>().Get() on fld.FolderID equals afm.FolderID
                                join acn in this._unitOfWork.RepositoryAsync<Account>().Get() on afm.AccountID equals acn.AccountID
                                where fi.FormDesignID == 7
                                select new DocumentCollateralViewModel
                                {
                                    TenantID = fld.TenantID,
                                    AccountID = acn.AccountID,
                                    AccountName = acn.AccountName,
                                    FolderID = fld.FolderID,
                                    ProductName = fi.Name,
                                    EffectiveDate = fldver.EffectiveDate,
                                    VersionNumber = fldver.FolderVersionNumber,
                                    Status = fldver.WorkFlowVersionState.WorkFlowState.WFStateName,
                                    FolderName = fld.Name,
                                    FolderVersionID = fldver.FolderVersionID,
                                    FormInstanceID = fi.FormInstanceID
                                }).ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return documentList;
        }
    }
}
