using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.config;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.reporting.Base.Interface;
using tmg.equinox.repository;
using tmg.equinox.repository.interfaces;
using tmg.equinox.repository.models;

namespace tmg.equinox.reporting.Base
{
    public class ReportRepository : IReportRepository
    {
        private IRptUnitOfWorkAsync _unitOfWork { get; set; }

        public ReportRepository()
        {
            _unitOfWork = new CoreRptUnitOfWork();
        }
        public ReportRepository(IRptUnitOfWorkAsync unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        public List<ReportSetting> Get()
        {
            if(_unitOfWork==null)
                _unitOfWork = new CoreRptUnitOfWork(); 
            return this._unitOfWork.RepositoryAsync<ReportSetting>().Get().ToList();                        
        }
    }
}
