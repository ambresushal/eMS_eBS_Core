using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;
using FolderVersionState = tmg.equinox.domain.entities.Enums.FolderVersionState;
using VersionType = tmg.equinox.domain.entities.Enums.VersionType;

namespace tmg.equinox.repository.extensions
{
    public static class FolderVersionRepository
    {
        public static bool IsAnyFolderVersionInProgress(this IRepositoryAsync<FolderVersion> folderVersionRepository, int folderId, int tenantId)
        {
            bool inProgress = false;
            

            //All FolderVersions should have FolderVersionState = Released / BaseLined and
            //FolderVersions should have (FolderVersionState = Retro and Released) or versionType = New
            inProgress = folderVersionRepository
                                 .Query()
                                 .Filter(fil => fil.FolderID == folderId && fil.TenantID == tenantId)
                                 .Get()
                                 .Any(fil => fil.FolderVersionStateID == (int)FolderVersionState.INPROGRESS);
            return inProgress;
        }

        public static bool IsValidFolderVersionNumber(this IRepositoryAsync<FolderVersion> folderVersionRepository,string versionNumber)
        {
            bool isValid = false;

            //Validate FolderVersionNumber if and only if this has format of 'XXXX_XX.X' or 'XXXX_XX.XX' 
            var regex = new Regex(@"^[0-9]{4}[_][0-9]{1,3}[.][0-9]{1,2}$");
            Match match = regex.Match(versionNumber);
            if (match.Success)
            {
                isValid = true;
            }
            return isValid;
        }

        public static bool IsFolderVersionInProgress(this IRepositoryAsync<FolderVersion> folderVersionRepository, int folderVersionId, int tenantId)
        {
            bool inProgress = false;
            inProgress = folderVersionRepository
                                 .Query()
                                 .Filter(fil => fil.FolderVersionID == folderVersionId && fil.TenantID == tenantId)
                                 .Get()
                                 .Any(fil => fil.FolderVersionStateID == (int)FolderVersionState.INPROGRESS);
            return inProgress;
        }
        public static bool IsFolderVersionInProgress(this IRepositoryAsync<FolderVersion> folderVersionRepository, int folderID, int folderVersionID, int tenantID)
        {
            bool inProgress = false;

            inProgress = folderVersionRepository
                                 .Query()
                                 .Filter(fil => fil.TenantID == tenantID && fil.FolderID == folderID && fil.FolderVersionID == folderVersionID)
                                 .Get()
                                 .Any(fil => fil.FolderVersionStateID != (int)FolderVersionState.RELEASED);
            return inProgress;
        }

    }
}
