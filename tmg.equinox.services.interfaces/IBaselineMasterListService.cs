using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IBaselineMasterListService
    {
        ServiceResult CreateBaseLineFolderBeforeUpdate(FolderVersionViewModel ViewModel, string Comment, string Username, bool isAsyncCall = true);

        string GetNextMinorVersionNumber(string folderVersionNumber, DateTime effectiveDate);
    }
}
