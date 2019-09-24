using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using tmg.equinox.core.logging.Logging;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.emailnotification.Interface;
using tmg.equinox.emailnotification.Model;
using tmg.equinox.net.smtp;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.emailnotification
{
    public class EmailNotificationManager : IEmailNotificationManager
    {
        private EmailNotificationRepository _emailNotificationRepository;
        EmailNotificationInfo _emailNotificationInfo;
        EmailNotificationQueue _emailNotificationQueue;
        private IUnitOfWorkAsync _unitOfWork;
        private static readonly ILog _logger = LogProvider.For<EmailNotificationRepository>();


        public EmailNotificationManager(Func<string, IUnitOfWorkAsync> unitOfWork)
        {
            _unitOfWork = unitOfWork("Email");
            _emailNotificationRepository = new EmailNotificationRepository(null, unitOfWork);
        }

        public void PrepareEmailTemplate(EmailNotificationInfo emailNotificationInfo)
        {
            _emailNotificationInfo = emailNotificationInfo;

            EmailTemplate emailTemplate = _emailNotificationRepository.GetEmailTemplate(_emailNotificationInfo.TemplateInfo.TemplateName);

            EmailNotificationQueue emailNotificationQueue = new EmailNotificationQueue();
            emailNotificationQueue.CreatedDateTime = DateTime.Now;
            emailNotificationQueue.ToBeSendDateTime = _emailNotificationInfo.ToBeSendDateTime;
            emailNotificationQueue.EmailSubject = PrepareEmailSubject(emailTemplate.EmailSubject);
            emailNotificationQueue.EmailBody = PrepareEmailContent(emailTemplate.EmailContent);
            emailNotificationQueue.FromAddress = _emailNotificationInfo.FromAddress;
            emailNotificationQueue.FromDisplayName = _emailNotificationInfo.FromDisplayName;
            emailNotificationQueue.ToAddresses = string.Join(",", _emailNotificationInfo.ToAddresses);

            if (_emailNotificationInfo.CCAddresses != null)
                emailNotificationQueue.CCAddresses = string.Join(",", _emailNotificationInfo.CCAddresses);

            if (_emailNotificationInfo.BCCAddresses != null)
                emailNotificationQueue.BCCAddresses = string.Join(",", _emailNotificationInfo.BCCAddresses);

            emailNotificationQueue.EmailImportance = _emailNotificationInfo.IsImportance;

            if (_emailNotificationInfo.Attachments != null)
                emailNotificationQueue.Attachments = string.Join(",", _emailNotificationInfo.Attachments);

            emailNotificationQueue.Source = _emailNotificationInfo.Source;
            emailNotificationQueue.SourceDescription = _emailNotificationInfo.SourceDescription;
            emailNotificationQueue.IsHTML = _emailNotificationInfo.IsHTML != null ? (bool)_emailNotificationInfo.IsHTML : emailTemplate.IsHTML;
            emailNotificationQueue.Status = "New";

            _emailNotificationQueue = emailNotificationQueue;
        }

        private string PrepareEmailSubject(string emailSubject)
        {
            foreach (var placeHolder in _emailNotificationInfo.TemplateInfo.PlaceHolder)
                emailSubject = emailSubject.Replace(placeHolder.PlaceHolderKey, placeHolder.PlaceHolderValue);

            return emailSubject;
        }

        private string PrepareEmailContent(string emailContent)
        {
            foreach (var placeHolder in _emailNotificationInfo.TemplateInfo.PlaceHolder)
                emailContent = emailContent.Replace(placeHolder.PlaceHolderKey, placeHolder.PlaceHolderValue);

            return emailContent;
        }

        public void UpdateEmailNotificationQueue()
        {
            _emailNotificationRepository.UpdateEmailNotificationQueue(_emailNotificationQueue);
        }

        public void SendEmailNotifications(ISmtpEmailSender emailSendar)
        {
            List<EmailNotificationQueue> emailNotificationQueue = _emailNotificationRepository.GetEmailNotificationQueue();
            var option = new TransactionOptions();
            option.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            option.Timeout = TimeSpan.FromMinutes(5);


            foreach (EmailNotificationQueue singleQueue in emailNotificationQueue)
            {
                Thread.Sleep(5000);
                _logger.Debug(string.Format("singleQueue : {0}", singleQueue.ID));
                using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, option))
                {

                    var queue = _emailNotificationRepository.GetEmailNotificationQueue(singleQueue.ID);

                    if (queue != null)
                    {
                        try
                        {
                            _logger.Debug(string.Format("queue : {0}", queue.ID));
                            if (queue.FromAddress == null)
                                emailSendar.Send(queue.ToAddresses, queue.EmailSubject, queue.EmailBody, queue.IsHTML);
                            else
                                emailSendar.Send(queue.FromAddress, queue.ToAddresses, queue.EmailSubject, queue.EmailBody, queue.IsHTML);
                        }

                        catch (Exception ex)
                        {
                            _logger.ErrorException(ex.Message, ex);
                            UpdateEmailNotificationQueueHistory(queue, ex);
                        }
                        finally
                        {
                            UpdateEmailNotificationQueueHistory(queue);
                        }
                    }
                    tran.Complete();
                    _logger.Info("Transaction scope has been Completed.");
                }
            }
        }

        private void UpdateEmailNotificationQueueHistory(EmailNotificationQueue queue)
        {
            EmailNotificationQueueHistory emailNotificationQueueHistory = PopulateEmailNotificationQueueHistory(queue);
            emailNotificationQueueHistory.SentDateTime = DateTime.Now;
            emailNotificationQueueHistory.Status = "Sent";
            queue.Status = emailNotificationQueueHistory.Status;
            _emailNotificationRepository.UpdateQueueInDatabase(queue);
            _emailNotificationRepository.UpdateQueueInDatabase(emailNotificationQueueHistory, queue);
        }

        private void UpdateEmailNotificationQueueHistory(EmailNotificationQueue queue, Exception exception)
        {
            EmailNotificationQueueHistory emailNotificationQueueHistory = PopulateEmailNotificationQueueHistory(queue);
            emailNotificationQueueHistory.SentDateTime = DateTime.Now;
            emailNotificationQueueHistory.Status = "Fail";
            emailNotificationQueueHistory.ErrorMessage = exception.Message;
            queue.Status = emailNotificationQueueHistory.Status;
            _emailNotificationRepository.UpdateQueueInDatabase(queue);
            _emailNotificationRepository.UpdateQueueInDatabase(emailNotificationQueueHistory, queue);
        }

        private EmailNotificationQueueHistory PopulateEmailNotificationQueueHistory(EmailNotificationQueue queue)
        {
            EmailNotificationQueueHistory emailNotificationQueueHistory = new EmailNotificationQueueHistory();
            emailNotificationQueueHistory.CreatedDateTime = queue.CreatedDateTime;
            emailNotificationQueueHistory.ToBeSendDateTime = queue.ToBeSendDateTime;
            emailNotificationQueueHistory.EmailSubject = queue.EmailSubject;
            emailNotificationQueueHistory.EmailBody = queue.EmailBody;
            emailNotificationQueueHistory.FromAddress = queue.FromAddress;
            emailNotificationQueueHistory.FromDisplayName = queue.FromDisplayName;
            emailNotificationQueueHistory.ToAddresses = queue.ToAddresses;
            emailNotificationQueueHistory.CCAddresses = queue.CCAddresses;
            emailNotificationQueueHistory.BCCAddresses = queue.BCCAddresses;
            emailNotificationQueueHistory.EmailImportance = queue.EmailImportance;
            emailNotificationQueueHistory.Attachments = queue.Attachments;
            emailNotificationQueueHistory.Source = queue.Source;
            emailNotificationQueueHistory.SourceDescription = queue.SourceDescription;
            emailNotificationQueueHistory.IsHTML = queue.IsHTML;
            emailNotificationQueueHistory.Status = queue.Status;
            return emailNotificationQueueHistory;
        }

    }
}
