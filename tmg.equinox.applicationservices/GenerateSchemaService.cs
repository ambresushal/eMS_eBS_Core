using System;
using System.Collections.Generic;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.repository.interfaces;
using tmg.equinox.schema.Base;
using tmg.equinox.schema.Base.Interface;
using tmg.equinox.schema.Base.Model;
using tmg.equinox.schema.sql;

namespace tmg.equinox.applicationservices
{
    public class GenerateSchemaService : IGenerateSchemaService
    {
        #region Private Memebers
        private ISchemaRepository _schemaRepostory;
        private IMDMSyncDataService _mDMSyncDataService;

        #endregion Private Memebers

        #region Public Properties

        #endregion Public Properties

        #region Constructor 

        public GenerateSchemaService(IRptUnitOfWorkAsync unitOfWork, IMDMSyncDataService mDMSyncDataService)
        {
            this._schemaRepostory = new SQLSchemaRepository(null, unitOfWork);
            this._mDMSyncDataService = mDMSyncDataService;
        }
        #endregion Constructor

        #region Public Methods

        public bool Run(List<JsonDesign> jsonDesigns)
        {
            SchemaGenerator schemaGenerator = new SchemaGenerator(this._schemaRepostory, _mDMSyncDataService);
            return schemaGenerator.Execute(jsonDesigns);
        }

        #endregion Public Methods
    }

   
}
