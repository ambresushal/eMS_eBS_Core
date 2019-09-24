using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.core.logging.Logging;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;
using tmg.equinox.setting.Interface;

namespace tmg.equinox.setting
{
    public class SettingRepository : ISettingRepository
    {
        private IUnitOfWorkAsync _unitOfWork;
        private static readonly core.logging.Logging.ILog _logger = core.logging.Logging.LogProvider.For<SettingRepository>();

        public SettingRepository(IUnitOfWorkAsync UnitOfWork)
        {
            _unitOfWork = UnitOfWork;
        }
        public List<SettingDefinition> GetSetting()
        {
            var definition = (from sd in _unitOfWork.RepositoryAsync<SettingDefinition>().Get()
                              select new
                              {
                                  sd.SettingName,
                                  sd.DefaultValue,
                                  sd.DisplayName,
                                  sd.SettingDefinitiontype
                              }).ToList();

            List<SettingDefinition> settingDefinition = new List<SettingDefinition>();
            foreach (var item in definition)
                settingDefinition.Add(new SettingDefinition(item.SettingName, item.DefaultValue, item.SettingDefinitiontype, item.DisplayName));

            return settingDefinition;
        }

        public List<SettingDefinition> GetSettingInfo()
        {
            var definition = (from sd in _unitOfWork.RepositoryAsync<SettingDefinition>().Get()
                              select new
                              {
                                  sd.SettingName,
                                  sd.DefaultValue,
                                  sd.DisplayName,
                                  sd.SettingDefinitiontype
                              }).ToList();

            List<SettingDefinition> settingDefinition = new List<SettingDefinition>();
            foreach (var item in definition)
                settingDefinition.Add(new SettingDefinition(item.SettingName, item.DefaultValue, item.SettingDefinitiontype, item.DisplayName));

            return settingDefinition;

        }
            

        public SettingDefinition Get(string name)
        {
            var definition = _unitOfWork.RepositoryAsync<SettingDefinition>().Get(m => m.SettingName == name).FirstOrDefault();

            return definition;
        }


        public void Add(SettingDefinition settingDefinition)
        {
            try
            {
                _unitOfWork.RepositoryAsync<SettingDefinition>().Insert(settingDefinition);
                _unitOfWork.Save();
            }
            catch(Exception ex)
            {
                _logger.ErrorException("Error while savig",ex);
            }
        }

        public void Update(SettingDefinition settingDefinition)
        {
            try
            {
                _unitOfWork.RepositoryAsync<SettingDefinition>().Update(settingDefinition);
                _unitOfWork.Save();
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Error while savig", ex);
            }
        }
    }
}
