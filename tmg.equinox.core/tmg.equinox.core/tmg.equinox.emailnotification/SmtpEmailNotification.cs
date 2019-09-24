using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
//using tmg.equinox.applicationservices.viewmodels.EmailNotitication;
using tmg.equinox.emailnotification.EmailAcknowledgement;
using tmg.equinox.emailnotification.model;
using tmg.equinox.emailnotification.Model;

namespace tmg.equinox.emailnotification
{
    public class SmtpEmailNotification
    {
        EmailResponseData responseData = new EmailResponseData();
        EmailAcknowledge emailStatus = new EmailAcknowledge();
        Task response;

        public EmailResponseData SendMessage(EmailSetting emailSettings)
        {
            try
            {
                MailMessage mailMessage = new MailMessage();
                emailSettings.To.ToList().ForEach(m => mailMessage.To.Add(m));
                mailMessage.From = emailSettings.SmtpFrom;
                mailMessage.Subject = emailSettings.SubjectLine;
                string text = emailSettings.Text;
                mailMessage.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Html));
                SmtpClient smtpClient = new SmtpClient(emailSettings.SmtpServerHostName, emailSettings.SmtpPort);
                // System.Net.NetworkCredential smtpCredentials = new System.Net.NetworkCredential(emailSettings.SmtpFrom.Address, emailSettings.SmtpPassword);
                // smtpClient.Credentials = smtpCredentials;
                smtpClient.Send(mailMessage);
               // responseData = emailStatus.GetAcknowledgement(response);

                
            }
            catch (Exception ex)
            {
                responseData.Message = ex.Message;
            }
            return responseData;
        }
    }
}
