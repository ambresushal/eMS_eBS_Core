using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FormDesign;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;
using System.Diagnostics.Contracts;
using System.Transactions;
using System.Text.RegularExpressions;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.applicationservices.viewmodels.PBPImport;
using tmg.equinox.domain.entities.Enums;

namespace tmg.equinox.applicationservices
{
    public class PBPMappingServices : IPBPMappingServices
    {
        #region Private Memebers
        private IUnitOfWorkAsync _unitOfWork { get; set; }
        #endregion
        #region Constructor
        public PBPMappingServices(IUnitOfWorkAsync unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        #endregion Constructor

        #region Public Method
        public void InitializeVariables(IUnitOfWorkAsync unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        public IEnumerable<PBPMappingViewModel> GetPBPViewMapList()
        {
            IEnumerable<PBPMappingViewModel> PBPViewMapList = null;

            PBPViewMapList = (from queue in this._unitOfWork.RepositoryAsync<PBPViewMap>()
                        .Get()
                        .Where(s => s.IsActive == true)
                              select new PBPMappingViewModel
                              {
                                  MappingId = queue.PBPViewMapID,
                                  ElementPath = queue.ElementPath,
                                  FieldPath = queue.FieldPath,
                                  PBPFieldName = queue.PBPFieldName,
                                  PBPTableName = queue.PBPTableName,
                                  CustomRuleTypeId = queue.CustomRuleTypeId,
                                  IsActive = queue.IsActive,
                                  IsCustomRule = queue.IsCustomRule,
                                  Year = queue.Year
                              }).ToList();

            return PBPViewMapList;
        }

        public IEnumerable<PBPMappingViewModel> GetMapping(string TableName, bool isFullMigration, string MappingType,int planYear)
        {
            IEnumerable<PBPMappingViewModel> MappingList = null;
            switch (MappingType)
            {
                case DocumentName.PBPVIEW:
                    MappingList = GetPBPViewMapListByTableName(TableName, isFullMigration);
                    break;
                case DocumentName.MEDICARE:
                    MappingList = GetPBPMedicareMapListByTableName(TableName, isFullMigration);
                    break;
            }
            MappingList = MappingList.Where(s => s.Year.Equals(planYear)).ToList();
            return MappingList;
        }

        #endregion

        #region Private

        private IEnumerable<PBPMappingViewModel> GetPBPViewMapListByTableName(string TableName, bool isFullMigration)
        {
            IEnumerable<PBPMappingViewModel> PBPViewMapList = null;

            PBPViewMapList = (from queue in this._unitOfWork.RepositoryAsync<PBPViewMap>()
                        .Get()
                        .Where(s => s.IsActive == true
                              && s.PBPTableName.Equals(TableName)
                              //&& s.IsFullMigration.Equals(isFullMigration) // Not needed
                              )
                              select new PBPMappingViewModel
                              {
                                  MappingId = queue.PBPViewMapID,
                                  ElementPath = queue.ElementPath,
                                  FieldPath = queue.FieldPath,
                                  PBPFieldName = queue.PBPFieldName,
                                  PBPTableName = queue.PBPTableName,
                                  CustomRuleTypeId = queue.CustomRuleTypeId,
                                  IsActive = queue.IsActive,
                                  IsCustomRule = queue.IsCustomRule,
                                  Year = queue.Year,
                              }).ToList();

            return PBPViewMapList;
        }

        private IEnumerable<PBPMappingViewModel> GetPBPMedicareMapListByTableName(string TableName, bool isFullMigration)
        {
            IEnumerable<PBPMappingViewModel> PBPViewMapList = null;

            PBPViewMapList = (from queue in this._unitOfWork.RepositoryAsync<PBPMedicareMap>()
                        .Get()
                        .Where(s => s.IsActive == true
                              && s.PBPTableName.Equals(TableName)
                              && s.IsFullMigration.Equals(isFullMigration)
                              )
                              select new PBPMappingViewModel
                              {
                                  MappingId = queue.PBPMedicareMapID,
                                  ElementPath = queue.ElementPath,
                                  FieldPath = queue.FieldPath,
                                  PBPFieldName = queue.PBPFieldName,
                                  PBPTableName = queue.PBPTableName,
                                  CustomRuleTypeId = queue.CustomRuleTypeId,
                                  IsActive = queue.IsActive,
                                  IsCustomRule = queue.IsCustomRule,
                                  Year = queue.Year,
                              }).ToList();

            return PBPViewMapList;
        }

        #endregion

    }
}
