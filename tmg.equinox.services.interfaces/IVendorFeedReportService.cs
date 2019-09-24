﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.VendorFeedReport;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IVendorFeedReportService
    {
        /// <summary>
        /// Gets the customer care document list.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>       
        /// <param name="groupName">The group name.</param>
        /// <param name="vendorName">The vendor name.</param>
        /// <param name="vendorType">The vendor type.</param>
        /// <param name="effectiveDate">The effective date of folder version.</param>
        /// <returns></returns>
        IEnumerable<VendorFeedReportViewModel> GetVendorFeedReportList(int tenantId, string vendorName, string groupName, string vendorType, DateTime effectiveDate, GridPagingRequest gridPagingRequest, out int totalCount);
        List<VendorFeedViewModel> GetVendorSectionName(int formInstanceID);
        List<VendorOutboundEligibilityViewModel> GetVendorOutboundSectionName(int formInstanceID);
        List<VendorOutboundClaimsViewModel> GetVendorOutboundClaimsSectionName(int formInstanceID);
        List<VendorInboundFileContractInfoViewModel> GetVendorInboundFileContractInfo(int formInstanceID);
    }
}
