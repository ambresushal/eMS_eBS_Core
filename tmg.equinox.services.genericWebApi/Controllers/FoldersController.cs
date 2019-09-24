using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.services.genericWebApi.Areas.Help.Model;

namespace tmg.equinox.services.genericWebApi.Controllers
{
    [Authorize]
    public class FoldersController : ApiController
    {
        /// <summary>
        /// Gets all folders data from the eBenefitSync.
        /// </summary>
        [HttpGet]
        public IHttpActionResult Get()
        {
            Folders fldr = new Folders();
            IList<Folders> folderList = fldr.GetFolderList();
            if (folderList == null)
                return BadRequest("Folders Not Found!");
            else
                return Ok(folderList);
        }
        /// <summary>
        /// Gets specific folder data from the eBenefitSync.
        /// </summary>
        [HttpGet]
        [ActionName("Folder")]
        public IHttpActionResult Get(int id)
        {
            Folders fldr = new Folders();
            IList<Folders> folderList = fldr.GetFolderList(id);
            if (folderList == null || folderList.Count == 0)
                return BadRequest("Folder Not Found!");
            else
                return Ok(folderList);
        }
        /// <summary>
        /// Gets specific folder version data from the eBenefitSync.
        /// </summary>
        [HttpGet]
        [ActionName("FolderVersion")]
        public IHttpActionResult FolderVersion(int id)
        {
            Folders fldr = new Folders();
            IList<FolderVersionAPIViewModel> folderVersionList = fldr.GetFolderVersions(id);
            if (folderVersionList == null || folderVersionList.Count == 0)
                return BadRequest("Folder Not Found!");
            else
                return Ok(folderVersionList);
        }
        /// <summary>
        /// Creates the folder with version.
        /// </summary>
        [HttpPost]
        [ActionName("AddFolder")]
        public IList<Account> PostAddFolder(int? accountID, string folderName, DateTime folderEffectiveDate, string primaryContact, string addedBy)
        {
            if(string.IsNullOrEmpty(folderName) || string.IsNullOrEmpty(primaryContact) || string.IsNullOrEmpty(addedBy))
                return new[] { new Account { ID = "", Errors = "Please enter Folder Name, Folder Effective Date, primary Contact and Added By!", success = false } };

            Folders addFolder = new Folders();
            bool isPortfolio = false;
            if (accountID == null)
                isPortfolio = true; 
            ServiceResult isFolderAdded = addFolder.AddFolder(accountID, folderName, folderEffectiveDate, isPortfolio, primaryContact, addedBy);
            string folderID = string.Empty;
            bool result = false;
            string errorMessage = null;
            if (isFolderAdded != null && isFolderAdded.Result == ServiceResultStatus.Success)
            {
                folderID = isFolderAdded.Items.FirstOrDefault().Messages[2];
                result = true;
                errorMessage = string.Empty;
            }
            else
            {
                errorMessage = "Error Occured While Folder Adding!";
            }
            return new[] { new Account { ID = folderID, Errors = errorMessage, success = result } };
        }
    }
}