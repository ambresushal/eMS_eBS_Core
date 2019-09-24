using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.Portfolio;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.repository.extensions;
using tmg.equinox.repository.interfaces;
using System.Diagnostics.Contracts;

namespace tmg.equinox.applicationservices
{
    public class PortfolioService : IPortfolioService
    {
        #region Private Member

        private IUnitOfWorkAsync _unitOfWork { get; set; }
        private ILoggingService _loggingService { get; set; }

        #endregion

        #region Constructor

        public PortfolioService(IUnitOfWorkAsync unitOfWork, ILoggingService loggingService)
        {
            this._unitOfWork = unitOfWork;
            this._loggingService = loggingService;
        }

        #endregion
        #region Public Methods

        /// <summary>
        /// Gets the Portfolio list.
        /// </summary>
        /// <param name="tenantId"> tenant identifier.</param>
        /// <returns></returns>
        public IEnumerable<PortfolioViewModel> GetPortfolioDetailsList(int tenantId)
        {
            //Contract.Requires(tenantId > 0, "Invalid tenantId");

            var result = new List<PortfolioViewModel>();
            var detailsWithCommaSeparatedList = new List<PortfolioViewModel>();
            //try
            //{
            //    List<PortfolioViewModel> portfolioDetailsList = (from fol in this._unitOfWork.RepositoryAsync<Folder>().Query().Include(c => c.AccountFolderMaps).Get()
            //                                                     join fldv in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
            //                                                         on fol.FolderID equals fldv.FolderID
            //                                                         into obj
            //                                                     from p in obj.DefaultIfEmpty()
            //                                                     join apm in this._unitOfWork.RepositoryAsync<AccountProductMap>().Get()
            //                                                         on p.FolderVersionID equals apm.FolderVersionID
            //                                                         into temp
            //                                                     from c in temp.DefaultIfEmpty()
            //                                                     where (fol.TenantID == tenantId && fol.IsPortfolio == true)
            //                                                     select new PortfolioViewModel
            //                                                         {
            //                                                             FolderName = fol.Name,
            //                                                             FolderID = fol.FolderID,
            //                                                             AccountID = fol.AccountFolderMaps.Select(d => d.AccountID).FirstOrDefault(),
            //                                                             MarketSegmentID = fol.MarketSegmentID,
            //                                                             PrimaryContactID = fol.PrimaryContentID,
            //                                                             FolderVersionID = p.FolderVersionID,
            //                                                             VersionNumber = p.FolderVersionNumber,
            //                                                             EffectiveDate = p.EffectiveDate,
            //                                                             Status = p.WorkFlowState.WFStateName,
            //                                                             ApprovalStatusID = p.FolderVersionWorkFlowStates.Where(s => s.FolderVersionID == p.FolderVersionID && s.IsActive == true && s.WFStateID == p.WFStateID).Select(x => x.ApprovalStatusID).FirstOrDefault(),
            //                                                             ProductName = c.ProductName,
            //                                                             ProductType = c.ProductType,
            //                                                             FolderVersionCount = fol.FolderVersions.Count(),
            //                                                             TenantID = tenantId
            //                                                         }).OrderByDescending(o => o.FolderVersionID).ToList();
            //    //Calling method from CommaSeparatedProductItems class to get comma-separated Product Types/Names
            //    this._commaSeparatedProductItems = new CommaSeparatedProductItems(portfolioDetailsList);
            //    detailsWithCommaSeparatedList = _commaSeparatedProductItems.GetCommaSeparatedListForPortfolio();

            //    //To find number of times the portfolio folder has been copied
            //    var usesCountList = (from f in this._unitOfWork.RepositoryAsync<Folder>().Get().ToList()
            //                         join p in detailsWithCommaSeparatedList
            //                         on f.ParentFolderId equals p.FolderID
            //                         group f by f.ParentFolderId into g
            //                         select new PortfolioViewModel
            //                         {
            //                             UsesCount = g.Count(),
            //                             ParentFolderId = g.Key
            //                         }).ToList();

            //    //Left join of DetailsWithCommaSeparatedList and usesCountList
            //    result = (from pl in detailsWithCommaSeparatedList
            //              join u in usesCountList
            //              on pl.FolderID equals u.ParentFolderId
            //              into pdlList
            //              from pdlListGrp in pdlList.DefaultIfEmpty()
            //              select new PortfolioViewModel
            //              {
            //                  FolderName = pl.FolderName,
            //                  FolderID = pl.FolderID,
            //                  FolderVersionID = pl.FolderVersionID,
            //                  AccountID = pl.AccountID,
            //                  MarketSegmentID = pl.MarketSegmentID,
            //                  PrimaryContactID = pl.PrimaryContactID,
            //                  VersionNumber = pl.VersionNumber,
            //                  EffectiveDate = pl.EffectiveDate,
            //                  Status = pl.Status,
            //                  ApprovalStatusID = pl.ApprovalStatusID,
            //                  ProductName = pl.ProductName,
            //                  ProductType = pl.ProductType,
            //                  FolderVersionCount = pl.FolderVersionCount,
            //                  UsesCount = (pdlListGrp != null && pdlListGrp.UsesCount.HasValue) ? pdlListGrp.UsesCount : null,
            //                  TenantID = tenantId
            //              }).ToList();

            //    if (result.Count() == 0)
            //        result = null;
            //}

            //catch (Exception ex)
            //{
            //    bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
            //    if (reThrow)
            //        throw;
            //}

            return result;
        }

        #endregion
    }
}
