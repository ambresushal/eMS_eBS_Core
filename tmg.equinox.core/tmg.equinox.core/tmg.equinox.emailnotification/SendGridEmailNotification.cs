using SendGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
//using tmg.equinox.applicationservices.viewmodels.EmailNotitication;
using tmg.equinox.emailnotification.EmailAcknowledgement;
using tmg.equinox.emailnotification.model;
using tmg.equinox.emailnotification.Model;

namespace tmg.equinox.emailnotification
{
    public class SendGridEmailNotification
    {
        EmailResponseData responseData = new EmailResponseData();
        EmailAcknowledge emailStatus = new EmailAcknowledge();
        Task response;

        public EmailResponseData SendMessage(EmailSetting emailSettings)
        {
            
            try
            {
                SendGridMessage sendGridMessage = new SendGridMessage();
                sendGridMessage.AddTo(emailSettings.To);
                sendGridMessage.From = emailSettings.SendGridFrom;
                sendGridMessage.Subject = emailSettings.SubjectLine;
                sendGridMessage.Html = emailSettings.Text;
                var sendGridcredentials = new NetworkCredential(emailSettings.SendGridFrom.Address, emailSettings.SendGridPassword);
                var transportWeb = new Web(sendGridcredentials);
                response = transportWeb.DeliverAsync(sendGridMessage);
                responseData=emailStatus.GetAcknowledgement(response);
                
                //Return success result
            }
            catch (Exception ex)
            {
                responseData.Message = ex.Message;
            }

            return responseData;
        }
    }
}
