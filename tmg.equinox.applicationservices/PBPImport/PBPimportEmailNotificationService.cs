using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.PBPImport;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.emailnotification.Interface;
using tmg.equinox.emailnotification.Model;
using tmg.equinox.net.smtp;
using tmg.equinox.repository.interfaces;
using tmg.equinox.setting;
using tmg.equinox.setting.Common;
using tmg.equinox.setting.Interface;

namespace tmg.equinox.applicationservices.PBPImport
{
   public class PBPImportEmailNotificationService: IPBPImportEmailNotificationService

    {
        private IPBPImportService _PBPImportService { get; set; }
        private IUnitOfWorkAsync _unitOfWork;

        public PBPImportEmailNotificationService(IUnitOfWorkAsync _unitOfWork)
        {
            this._unitOfWork = _unitOfWork;
            //_PBPImportService = new PBPImportService(this._unitOfWork);
        }

        public void Execute()
        {
            _PBPImportService.SendPBPImportEmail();
        }
    }
}
