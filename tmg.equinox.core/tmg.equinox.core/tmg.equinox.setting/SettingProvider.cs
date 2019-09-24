using System;
using System.Collections.Generic;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;
using tmg.equinox.setting.Interface;

namespace tmg.equinox.setting
{
    public class SettingProvider : ISettingProvider
    {
        ISettingRepository _settingRepository;
        private IUnitOfWorkAsync _unitOfWork;
        List<SettingDefinition> _settingDefinition;
        public SettingProvider(Func<string, IUnitOfWorkAsync> UnitOfWork)
        {
            _unitOfWork = UnitOfWork("Core");
            _settingRepository = new SettingRepository(_unitOfWork);
        }

        public List<SettingDefinition> GetSettingDefinitions()
        {
            if (_settingDefinition == null || _settingDefinition.Count <= 0)
                _settingDefinition = _settingRepository.GetSetting();

            return _settingDefinition;
        }
        public SettingDefinition Get(string name)
        {
            return _settingRepository.Get(name);
        }
        public void Add(SettingDefinition settingDefinition)
        {
            _settingRepository.Add(settingDefinition);
        }

        public void Update(SettingDefinition settingDefinition)
        {
            _settingRepository.Update(settingDefinition);
        }
        public List<SettingDefinition> GetSettingInfo()
        {
            if (_settingDefinition == null || _settingDefinition.Count <= 0)
                _settingDefinition = _settingRepository.GetSettingInfo();

            return _settingDefinition;
        }
    }
}
