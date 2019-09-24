using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.PrintTemplate;

namespace tmg.equinox.applicationservices.interfaces
{
   public interface ITemplateReportService
    {
       IEnumerable<TemplateViewModel> GetDocumentTemplateList(int tenantId);
       ServiceResult AddDocumentTemplate(int tenantId, int formDesignId, int formDesignVersionId, string templateName, string description, string userName);
       IEnumerable<TemplateViewModel> LoadTemplateDesignUIElement(int tenantId, int formDesignVersionId, int templateId);
       ServiceResult UpdateTemplateDesignUIElement(int tenantId, List<TemplateViewModel> templateUIElementList, int templateId, string userName);
       ServiceResult DeleteDocumentTemplate(int tenantId, int templateId);
    }
}
