using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.emailnotification.model
{
    public class EmailSetting
    {

        public EmailSetting()
        {
            this.DisplayName = "CBC@Support.com";
            //this.SendGridUserName = "azure_03962de6af16e20630e7ff49df7354e5@azure.com";
            //this.SendGridPassword = "I564rCSWH7LPzof";
            //this.SendGridFrom = new MailAddress(this.SendGridUserName, this.DisplayName);
            //this.SmtpUserName = "HelpDesk@themostgroup.com";
            //this.SmtpPassword = "admin@123";
            //this.SmtpPort = 25;
            //this.SmtpServerHostName = "mail.themostgroup.com";
            //this.SmtpFrom = new MailAddress(this.SmtpUserName, this.DisplayName);
        }

        public List<string> To { get; set; }
        public List<string> Cc { get; set; }
        public List<string> BCc { get; set; }
        public string SubjectLine { get; set; }
        public string Text { get; set; }
        public string Html { get; set; }
        public List<string> Attachments { get; set; }
        public string DisplayName { get; set; }
        public MailAddress SendGridFrom { get; set; }
        public string SendGridUserName { get; set; }
        public string SendGridPassword { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpPassword { get; set; }
        public string SmtpUserName { get; set; }
        public string SmtpServerHostName { get; set; }
        public MailAddress SmtpFrom { get; set; }
    }
}
