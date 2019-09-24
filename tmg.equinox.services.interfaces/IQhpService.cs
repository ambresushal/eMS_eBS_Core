using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IQhpService
    {
        ServiceResult AddTemplate(QhpUploadTemplateViewModel viewModel);
        List<FormInstanceViewModel> GetFormInstanceList(int tenantId, int folderVersionId, int folderId, int formDesignID);
    }
}
