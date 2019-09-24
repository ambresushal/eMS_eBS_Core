using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.emailnotification.Model;
using tmg.equinox.net.smtp;

namespace tmg.equinox.emailnotification.Interface
{
    public interface IEmailNotificationManager
    {
        void PrepareEmailTemplate(EmailNotificationInfo emailNotificationInfo);
        void UpdateEmailNotificationQueue();
        void SendEmailNotifications(ISmtpEmailSender emailSendar);

    }
}
