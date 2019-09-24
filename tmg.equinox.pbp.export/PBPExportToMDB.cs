using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.PBPImport;
using tmg.equinox.domain.entities;
using tmg.equinox.domain.entities.Models;
//using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.repository.interfaces;
using tmg.equinox.schema.Base;

namespace tmg.equinox.pbpexport
{
    public class PBPExportToMDB
    {
        private IUnitOfWorkAsync _unitOfWork { get; set; }
        private IPBPExportServices _pbpExportServices { get; set; }
        private IExitValidateService _evService { get; set; }
        static readonly object _lockobject = new object();

        public PBPExportToMDB(IUnitOfWorkAsync unitOfWork, IPBPExportServices pbpExportServices,IExitValidateService evService)
        {
            this._unitOfWork = unitOfWork;
            this._pbpExportServices = pbpExportServices;
            this._evService = _evService;
        }

        public void GenerateMDBFile(int PBPExportQueueID, string userName)
        {
            lock (_lockobject)
            {
                _pbpExportServices.GenerateMDBFile(PBPExportQueueID, userName);
            }
        }

        public string ExitValidateGenerateMDBFile(int ExitValidateQueueID, string userName)
        {
            return _pbpExportServices.ExitValidateGenerateMDBFile(ExitValidateQueueID, userName);
        }
    }
}
