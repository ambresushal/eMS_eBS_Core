using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.caching.Interfaces;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.reporting.Base.Interface;
using tmg.equinox.reporting.Base.Mapping;
using tmg.equinox.repository.models;

namespace tmg.equinox.reporting.Base
{
    public class ReportConfig : IReportConfig
    {
        List<ReportSetting> _reportSettings;
        ICacheProvider _cacheProvider;
        IReportRepository _reportRepository;
        IMapper<IList<ReportMappingField>> _mapper;
        IMapper<string> _SQLmapper;
        private const string CACHE_KEY_REPORTING_SETTING= "CACHE_KEY_REPORTING_SETTING";
        public ReportConfig( IReportRepository reportRepository, IMapper<IList<ReportMappingField>> mapper, IMapper<string> SQLmapper)
        {
            //_cacheProvider = cacheProvider;
            _reportRepository = reportRepository;
            _mapper = mapper;
            _SQLmapper = SQLmapper;

        }
        
        private void getReportSettings()
        {

            if (_cacheProvider != null)
            {
                _reportSettings = _cacheProvider.Get<List<ReportSetting>>(CACHE_KEY_REPORTING_SETTING);
                if (_reportSettings == null)
                    _reportSettings = _reportRepository.Get().ToList();
                    _cacheProvider.Add<List<ReportSetting>>(CACHE_KEY_REPORTING_SETTING, _reportSettings);
            }
            else
                _reportSettings = _reportRepository.Get().ToList();
               
                //foreach (var config in _reportSettings)
                //{
                //    config.mappings = _mapper.GetMapping();
                //    _SQLmapper = new SQLMapper(config);
                //    config.SQLstatement = _SQLmapper.GetMapping();
                //}
                          
        }

        public ReportSetting GetReportSettingByName(string reportName)
        {
            getReportSettings();
            var result = _reportSettings.ToList().Where(r => r.ReportName.Equals(reportName)).FirstOrDefault();
            return result;
        }

        public ReportSetting GetReportSettingById(int reportId)
        {
            getReportSettings();
            string CurrentYear = DateTime.Now.Year.ToString();
            string NextYear = ((DateTime.Now.Year) + 1).ToString();
            //var result = this._reportRepository.Get().ToList().Where(r => r.ReportId.Equals(reportId)).FirstOrDefault();
            //return result;
            var result = (from r in _reportSettings.Where(r => r.ReportId.Equals(reportId))
                          select new ReportSetting
                          {
                              ReportId = r.ReportId,
                              ReportName = r.ReportName.Replace("2018", CurrentYear).Replace("2019", NextYear),
                              ReportTemplatePath = r.ReportTemplatePath,
                              OutputPath = r.OutputPath,
                              ReportNameFormat = r.ReportNameFormat,
                              Description = r.Description,
                              SQLstatement = r.SQLstatement.Replace(@"\r\n", ""),
                          }).FirstOrDefault();
            return result;
        }

        //send it without mapping & sql
        public List<ReportSetting> GetReportSetting()
        {
            getReportSettings();            
            return _reportSettings.ToList();
        }
    }
}
