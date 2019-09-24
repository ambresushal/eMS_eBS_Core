using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using tmg.equinox.caching.Interfaces;
using tmg.equinox.core.logging.Logging;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.emailnotification.Interface;
using tmg.equinox.emailnotification.Model;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.emailnotification
{
    public class EmailNotificationRepository : IEmailNotificationRepository
    {
        private IUnitOfWorkAsync _unitOfWork;
        private static readonly ILog _logger = LogProvider.For<EmailNotificationRepository>();
        private readonly object _lockObject = new Object();

        public EmailNotificationRepository(ICacheProvider cacheProvider, Func<string, IUnitOfWorkAsync> unitOfWork)
        {
            _unitOfWork = unitOfWork("Email");
        }

        public EmailTemplateInfo GetEmailTemplateInfo(string templateName)
        {
            EmailTemplateInfo emailTemplateInfo = new EmailTemplateInfo();
            var lstPlaceHpolders = (from ETPHM in _unitOfWork.RepositoryAsync<EmailTemplatePlaceHolderMapping>().Get()
                                    join ETPH in _unitOfWork.RepositoryAsync<EmailTemplatePlaceHolder>().Get() on ETPHM.PlaceHolderId equals ETPH.ID
                                    join ET in _unitOfWork.RepositoryAsync<EmailTemplate>().Get() on ETPHM.EmailTemplateId equals ET.ID
                                    where (ET.TemplateName == templateName)
                                    select new
                                    {
                                        TemplateName = ET.TemplateName,
                                        PlaceHolder = ETPH.PlaceHolder
                                    }).ToList();

            if (lstPlaceHpolders.Count > 0)
                emailTemplateInfo.TemplateName = lstPlaceHpolders[0].TemplateName;

            List<EmailTemplatePlaceHolderInfo> lstpH = new List<EmailTemplatePlaceHolderInfo>();
            foreach (var objPlaceHpolder in lstPlaceHpolders)
            {
                EmailTemplatePlaceHolderInfo emailTemplatePlaceHolderInfo = new EmailTemplatePlaceHolderInfo();
                emailTemplatePlaceHolderInfo.PlaceHolderKey = objPlaceHpolder.PlaceHolder;
                lstpH.Add(emailTemplatePlaceHolderInfo);
            }
            emailTemplateInfo.PlaceHolder = lstpH;

            return emailTemplateInfo;
        }

        public EmailTemplate GetEmailTemplate(string templateName)
        {
            return _unitOfWork.RepositoryAsync<EmailTemplate>().Get().Where(x => x.TemplateName == templateName).FirstOrDefault();
        }



        public void UpdateEmailNotificationQueue(EmailNotificationQueue _emailNotificationQueue)
        {
            try
            {
                _unitOfWork.RepositoryAsync<EmailNotificationQueue>().Insert(_emailNotificationQueue);
                _unitOfWork.Save();
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Exception occurred while email notification queue.", ex);
                throw ex;
            }
        }

        public void UpdateQueueInDatabase(EmailNotificationQueueHistory emailNotificationQueueHistory, EmailNotificationQueue queue)
        {
            var option = new TransactionOptions();
            option.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            option.Timeout = TimeSpan.FromMinutes(5);
            try
            {
                _unitOfWork.RepositoryAsync<EmailNotificationQueueHistory>().Insert(emailNotificationQueueHistory);
                _unitOfWork.RepositoryAsync<EmailNotificationQueue>().Delete(queue);
                _unitOfWork.Save();
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Exception occures while updating Email Notification Queue in database", ex);
                throw ex;
            }
        }

        public void UpdateQueueInDatabase(EmailNotificationQueue queue)
        {
            _unitOfWork.RepositoryAsync<EmailNotificationQueue>().Update(queue);
            _unitOfWork.Save();
        }


        public List<EmailNotificationQueue> GetEmailNotificationQueue()
        {
            return _unitOfWork.RepositoryAsync<EmailNotificationQueue>().Get().Where(x => x.Status == "New" & x.ToBeSendDateTime <= DateTime.Now).ToList();
        }
        public EmailNotificationQueue GetEmailNotificationQueue(int EmailNotificationQueueId)
        {
            return _unitOfWork.RepositoryAsync<EmailNotificationQueue>().Get().Where(x => x.ID == EmailNotificationQueueId && x.Status == "New").FirstOrDefault();
        }


    }
}
