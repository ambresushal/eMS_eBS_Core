using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;
//using tmg.equinox.emailnotification.Model;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IEmailNotificationService
    {
        TEmailNotificationInfo GetEmailTemplateInfo<TEmailNotificationInfo>(EmailTemplateTypes templateType);
        void SendEmail<TEmailNotificationInfo>(TEmailNotificationInfo emailNotificationInfo);
        void Execute();
    }
}
