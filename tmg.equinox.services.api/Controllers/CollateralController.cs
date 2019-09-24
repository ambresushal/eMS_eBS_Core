using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using tmg.equinox.services.api.Framework;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.repository.interfaces;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.services.api.Controllers
{
    public class CollateralController : BaseApiController
    {
        private IUnitOfWorkAsync _unitOfWork { get; set; }
        private ICollateralService _collateralService;
        private IFormInstanceService _formInstanceService;
        
        public CollateralController(ICollateralService collateralService, IFormInstanceService formInstanceService, IUnitOfWorkAsync unitOfWork)
        {
            this._collateralService = collateralService;
            this._formInstanceService = formInstanceService;
            this._unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Download the document for plan by filetype
        /// </summary>
        /// <param name="formInstanceID"></param>
        /// <param name="reportName"></param>
        /// <param name="templateReportVersionID"></param>
        /// <param name="fileFormat"></param>
        /// <returns></returns>
        [HttpGet]
        [System.Web.Http.Description.ResponseType(typeof(File))]
        [Route("api/v1/Collateral/GetDocument/")]
        public IHttpActionResult GetDocumentByType(int formInstanceID, string reportName, int templateReportVersionID, string fileFormat)
        {
            bool doesFileExist = false;
            int processQueue1Up = (from queue in _unitOfWork.RepositoryAsync<CollateralProcessQueue>().Get()
                           where queue.FormInstanceID == formInstanceID && queue.TemplateReportVersionID == templateReportVersionID
                           select queue.CollateralProcessQueue1Up).FirstOrDefault();
                        
            string filePath = _collateralService.GetFilePath(processQueue1Up, fileFormat);
            if(filePath != "")
            {
                string path = filePath.Split('|')[0] + filePath.Split('|')[1] + "." + fileFormat;
                if (System.IO.File.Exists(path))
                    doesFileExist = true;

                var fileAndFolderPath = filePath.Split('|');
                FileStream result = System.IO.File.OpenRead(path);

                byte[] dataBytes = new byte[result.Length];
                result.Read(dataBytes, 0, (int)result.Length);

                string description = "File has been downloaded against template " + reportName;
                string username = "superuser";
                _collateralService.SaveTemplateActivityLog(templateReportVersionID, description, username);

                string documentName = "CollateralFile";
                if (fileAndFolderPath != null && fileAndFolderPath.Length > 1 && !String.IsNullOrEmpty(fileAndFolderPath[1]))
                    documentName = fileAndFolderPath[1];
                var dataStream = new MemoryStream(dataBytes);

                if (doesFileExist)
                    return new documentResult(dataStream, Request, documentName, fileFormat);
            }
            return NotFound();
        }

        /// <summary>
        /// Queue Document
        /// </summary>
        /// <param name="formInstanceIDs"></param>
        /// <param name="reportTemplateID"></param>
        /// <param name="reportName"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v1/Collateral/QueueCollateral/")]
        public HttpResponseMessage QueueCollateral(string formInstanceIDs, int reportTemplateID, string reportName, string userName)
        {
            int formInstanceID = Convert.ToInt32(formInstanceIDs);
            var formInstanceData = _formInstanceService.GetFormInstanceDetails(formInstanceID);

            string folderVersionEffDt = Convert.ToString(formInstanceData.EffectiveDate).Split(' ')[0];
            IEnumerable<string> formInstanceIDList = formInstanceData.FormInstanceID.ToString().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            IEnumerable<string> folderVersionNumberArr = formInstanceData.FolderVersionNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            IEnumerable<string> productIdArr = formInstanceData.FormName.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            string collateralFolderPath = "D:\\CollateralEngine\\";
            ServiceResult result = _collateralService.QueueCollateral(0, "undefined", formInstanceData.FolderId, formInstanceData.FolderName, formInstanceData.FolderVersionID, formInstanceIDList, folderVersionNumberArr, productIdArr, reportTemplateID, folderVersionEffDt, userName, null, collateralFolderPath, reportName);
            if (result == null)
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Queue Failed !!!");
            var response = Request.CreateResponse(HttpStatusCode.OK, "Document queued successfully !!!");            
            return response;
        }
    }

    public class documentResult : IHttpActionResult
    {
        MemoryStream documentDataStream;
        string fileName;
        HttpRequestMessage httpRequestMessage;
        HttpResponseMessage httpResponseMessage;
        public documentResult(MemoryStream data, HttpRequestMessage request, string documentName, string fileFormat)
        {
            documentDataStream = data;
            httpRequestMessage = request;
            fileName = documentName + "." + fileFormat;
        }
        public System.Threading.Tasks.Task<HttpResponseMessage> ExecuteAsync(System.Threading.CancellationToken cancellationToken)
        {
            httpResponseMessage = httpRequestMessage.CreateResponse(HttpStatusCode.OK);
            httpResponseMessage.Content = new StreamContent(documentDataStream);            
            httpResponseMessage.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            httpResponseMessage.Content.Headers.ContentDisposition.FileName = fileName;
            httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

            return System.Threading.Tasks.Task.FromResult(httpResponseMessage);
        }
    }    
}