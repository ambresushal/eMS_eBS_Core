using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.Account;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.dependencyresolution;
using tmg.equinox.infrastructure.exceptionhandling;

namespace tmg.equinox.services.genericWebApi.Areas.Help.Model
{
    public class Folders
    {
        private IFolderVersionServices _serviceFolder { get; set; }
        private IConsumerAccountService _serviceAccount { get; set; }
        private IAccountService _serviceUserAccount { get; set; }

        public string FolderID { get; set; }
        public  string FolderName { get; set; }
        public string PrimaryContact { get; set; }
        public string AccountName { get; set; }
        public IList<url> url { get; set; }
        private int tenantId = 1;
        public Folders()
        {
            _serviceFolder = UnityConfig.Resolve<IFolderVersionServices>();
            _serviceAccount = UnityConfig.Resolve<IConsumerAccountService>();
            _serviceUserAccount = UnityConfig.Resolve<IAccountService>();
        }
        public List<Folders> GetFolderList()
        {
            List<Folders> lstfolder = null;
            try 
            {
                lstfolder = (from fd in this._serviceFolder.GetAllFolderList(tenantId)
                          select new Folders
                          {
                              FolderID = fd.FolderId.ToString(),
                              FolderName = fd.FolderName,
                              PrimaryContact = fd.PrimaryContact,
                              AccountName = fd.AccountName,
                              url = (new[] { new url { folders = "/api/data/v1.0/Folders", rowTemplate = "/api/data/v1.0/Folders/Folder/" + fd.FolderId } }),
                          }).ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                return null;
            }
            return lstfolder;
        }
        public List<Folders> GetFolderList(int folderID)
        {
            var lstFolder = (from fd in this._serviceFolder.GetAllFolderList(tenantId)
                             where fd.FolderId == folderID
                             select new Folders
                             {
                                 FolderID = fd.FolderId.ToString(),
                                 FolderName = fd.FolderName,
                                 PrimaryContact = fd.PrimaryContact,
                                 AccountName = fd.AccountName,
                                 url = (new[] { new url { folders = "/api/data/v1.0/Folders", rowTemplate = "/api/data/v1.0/Folders/Folder/" + fd.FolderId } }),
                             }).ToList();
            return lstFolder;
        }
        public List<FolderVersionAPIViewModel> GetFolderVersions(int folderID)
        {
            var lstFolderVersions = (from fd in this._serviceFolder.GetFolderVersionByFolderId(folderID)
                                     select new FolderVersionAPIViewModel 
                                     { 
                                         ID = fd.ID,
                                         Version = fd.Version,
                                         EffectiveDate = fd.EffectiveDate,
                                         Status = fd.Status,
                                         Product = fd.Product,
                                     }).ToList();
            return lstFolderVersions;
        }
        public ServiceResult AddFolder(int? accountID, string folderName, DateTime folderEffectiveDate, bool isPortfolio, string primaryContact, string addedBy)
        {
            ServiceResult result = null;
            try
            {
                LoginViewModel userData = this._serviceUserAccount.FindUser(tenantId, addedBy);
                if (userData == null)
                    return null;
                //result = this._serviceAccount.AddFolder(tenantId, accountID, folderName, folderEffectiveDate, isPortfolio, userData.UserId, primaryContact, tenantId, null, null, null, null, addedBy);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                return null;
            }
            return result;
        }
    }
    public class url
    {
        public string folders { get; set; }
        public string rowTemplate { get; set; }
    }
}