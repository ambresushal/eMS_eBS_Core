using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.core.logging.Logging;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.applicationservices.MLImport
{
    public class BaselineMasterListService : IBaselineMasterListService
    {
        private IFolderVersionServices _folderVersionServices;
        private IUnitOfWorkAsync _unitOfWorkAsync;
        private static readonly ILog _logger = LogProvider.For<BaselineMasterListService>();

        public BaselineMasterListService(IFolderVersionServices folderVersionServices, IUnitOfWorkAsync unitOfWorkAsync)
        {
            this._folderVersionServices = folderVersionServices;
            this._unitOfWorkAsync = unitOfWorkAsync;
        }

        public ServiceResult CreateBaseLineFolderBeforeUpdate(FolderVersionViewModel ViewModel, string Comment, string Username, bool isAsyncCall = true)
        {
            ServiceResult Result = new ServiceResult();
            //VersionNumberBuilder builder = null;
            //builder = new VersionNumberBuilder();
            _logger.Debug("ml before CreateBaseLineFolderBeforeUpdate");
            string CurrentVersionNumber = this._unitOfWorkAsync.RepositoryAsync<FolderVersion>()
                                          .Get()
                                          .Where(s => s.FolderVersionID == ViewModel.FolderVersionId
                                                 && s.IsActive == true)
                                          .Select(s => s.FolderVersionNumber).FirstOrDefault();

            string NextFolderVersionName = this.GetNextMinorVersionNumber(CurrentVersionNumber, ViewModel.EffectiveDate);
            bool isRelease = false;
            if (ViewModel.FolderVersionStateID == 3 || ViewModel.FolderVersionStateID == 4)
            {
                isRelease = true;
            }
            Result = _folderVersionServices.BaseLineFolder(1, 0, ViewModel.FolderId, ViewModel.FolderVersionId,
            0, Username, NextFolderVersionName, Comment, 0, null, isRelease, false, true, isAsyncCall);
            _logger.Debug("ml after CreateBaseLineFolderBeforeUpdate");
            return Result;
        }


        public string GetNextMinorVersionNumber(string folderVersionNumber, DateTime effectiveDate)
        {
            _logger.Debug("ml before GetNextMinorVersionNumber");
            int _year = effectiveDate.Year;
            string _versionNumber = folderVersionNumber;
            bool _isMinor = true;

            const string UNDERSCORE = "_";
            const string NULLSTRING = "";
            const string FIRSTMINORVERSION = "0.01";
            const string MAJORVERSION = ".0";

            string nextVersionNumber = null;
            double versionNumeric = 0;
            if (!string.IsNullOrEmpty(_versionNumber))
            {

                if (_isMinor)
                {
                    if (_versionNumber.Split('_')[0] != _year.ToString())
                        nextVersionNumber = NULLSTRING + _year + UNDERSCORE + FIRSTMINORVERSION;
                    else
                    {
                        versionNumeric = Convert.ToDouble(_versionNumber.Split('_')[1]) + 0.01;
                        nextVersionNumber = _year.ToString() + UNDERSCORE + versionNumeric;
                    }
                }
                else
                {
                    string[] versionData = _versionNumber.Split('_');
                    versionNumeric = Convert.ToDouble(versionData[1].Split('.')[0]) + 1;
                    nextVersionNumber = _year.ToString() + UNDERSCORE + versionNumeric + MAJORVERSION;
                }
            }
            else
            {
                if (_year > 0)
                    nextVersionNumber = NULLSTRING + _year + UNDERSCORE + FIRSTMINORVERSION;
                else
                    nextVersionNumber = NULLSTRING + DateTime.Now.Year + UNDERSCORE + FIRSTMINORVERSION;
            }
            _logger.Debug("ml after GetNextMinorVersionNumber");
            return nextVersionNumber;
        }
    }
}
