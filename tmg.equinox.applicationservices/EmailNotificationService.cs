using System;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.core.logging.Logging;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.emailnotification;
using tmg.equinox.emailnotification.Interface;
using tmg.equinox.emailnotification.Model;
using tmg.equinox.net.smtp;
using tmg.equinox.repository.interfaces;
using tmg.equinox.setting;
using tmg.equinox.setting.Common;
using tmg.equinox.setting.Interface;

namespace tmg.equinox.emailnotification
{

    public class EmailNotificationService : IEmailNotificationService
    {
        private IEmailNotificationRepository _emailNotificationRepository;
        private IUnitOfWorkAsync _unitOfWork;
        private IEmailNotificationManager _emailNotificationManager;
        private ISettingProvider _settingProvider;
        private ISettingManager _settingManger;
        private ISmtpEmailSenderConfiguration _emailConfig;
        private ISmtpEmailSender _emailSendar;
        private static readonly ILog _logger = LogProvider.For<EmailNotificationRepository>();

        public EmailNotificationService(Func<string, IUnitOfWorkAsync> unitOfWork)
        {
            _unitOfWork = unitOfWork("Email");
            _emailNotificationRepository = new EmailNotificationRepository(null, unitOfWork);
            _emailNotificationManager = new EmailNotificationManager(unitOfWork);
            _settingManger = new SettingManager(_settingProvider);
            _emailConfig = new SmtpEmailSenderConfiguration(_settingManger);
            _emailSendar = new SmtpEmailSender(_emailConfig);
        }
        public EmailNotificationService(IUnitOfWorkAsync unitOfWork, Func<string, IUnitOfWorkAsync> _unitOfWorkAsync)
        {
            _unitOfWork = unitOfWork;
            _emailNotificationRepository = new EmailNotificationRepository(null, _unitOfWorkAsync);
            _emailNotificationManager = new EmailNotificationManager(_unitOfWorkAsync);
            _settingProvider = new SettingProvider(_unitOfWorkAsync);
            _settingManger = new SettingManager(_settingProvider);
            _emailConfig = new SmtpEmailSenderConfiguration(_settingManger);
            _emailSendar = new SmtpEmailSender(_emailConfig);
        }

        public TEmailNotificationInfo GetEmailTemplateInfo<TEmailNotificationInfo>(EmailTemplateTypes templateType)
        {
            return (TEmailNotificationInfo)Convert.ChangeType(_emailNotificationRepository.GetEmailTemplateInfo(templateType.ToString()), typeof(TEmailNotificationInfo));
        }

        public void SendEmail<TEmailNotificationInfo>(TEmailNotificationInfo emailNotificationInfo)
        {
            var emailNotification = (EmailNotificationInfo)Convert.ChangeType(emailNotificationInfo, typeof(EmailNotificationInfo));

            _emailNotificationManager.PrepareEmailTemplate(emailNotification);
            _emailNotificationManager.UpdateEmailNotificationQueue();
        }

        public void Execute()
        {
            try
            {
                _emailSendar.BuildClient();
                _emailNotificationManager.SendEmailNotifications(_emailSendar);
            }
            catch (Exception ex)
            {
                _logger.ErrorException(ex.Message, ex);
            }
        }

    }
}
