using System;
using System.Collections.Generic;
using tmg.equinox.applicationservices.viewmodels.Consortium;
using tmg.equinox.applicationservices.viewmodels;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IConsortiumService
    {
        /// <summary>
        /// Gets the list of Consortium.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <returns></returns>
        GridPagingResponse<ConsortiumViewModel> GetConsortiumList(int tenantID, GridPagingRequest gridPagingRequest);

        /// <summary>
        /// Adds the Consortium.
        /// </summary>
        /// <param name="tenantId">tenant identifier.</param>
        /// <param name="accountName">Name of the Consortium.</param>
        /// <param name="addedBy">added by.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Consortium Name already exists</exception>
        ServiceResult AddConsortium(string consortiumName, int tenantID, string addedBy);

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
        ServiceResult UpdateConsortium(int consortiumID, string consortiumName, int tenantID, string updatedBy);

        /// <summary>
        /// Gets the list of Consortium.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <returns></returns>
        IEnumerable<ConsortiumViewModel> GetConsortiumForDropdown(int tenantID);

        /// <summary>
        /// Gets details of a Consortium.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <returns></returns>
        ConsortiumViewModel GetConsortium(int folderVersionID);

        int? GetConsortiumId(string consortiumName);

    }
}
