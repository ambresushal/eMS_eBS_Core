using System;
using System.Collections.Generic;
using tmg.equinox.applicationservices.viewmodels.Portfolio;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IPortfolioService
    {
        /// <summary>
        /// Gets the Portfolio folder details list.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <returns></returns>
        IEnumerable<PortfolioViewModel>GetPortfolioDetailsList(int tenantId);
       
    }
}
