using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.emailnotification.Model;

namespace tmg.equinox.emailnotification.Interface
{
    public interface IEmailNotificationRepository
    {
        EmailTemplateInfo GetEmailTemplateInfo(string templateName);
        EmailTemplate GetEmailTemplate(string templateName);
    }
}
