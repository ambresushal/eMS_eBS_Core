using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.PBPImport;
using model = tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;
using tmg.equinox.web.ODMExecuteManager.Model;
using Newtonsoft.Json.Linq;

namespace tmg.equinox.web.MDBComparer
{
    public class MDBComparerHelper
    {
        public string FilePath = string.Empty;
        private IFolderVersionServices _folderVersionService { get; set; }
        private IUnitOfWorkAsync _unitOfWork { get; set; }
        public MDBComparerHelper(string importfile, IFolderVersionServices folderVersionService, IUnitOfWorkAsync unitOfWork)
        {
            FilePath = importfile;
            this._folderVersionService = folderVersionService;
            this._unitOfWork = unitOfWork;
        }

        public DataTable GetQIDs()
        {
            DataTable PBPDataTable = null;
            try
            {
                AccessDBTableService exportServiceObj = new AccessDBTableService(this.FilePath);
                PBPDataTable = exportServiceObj.ReadTable("PBP");
            }
            catch (Exception ex)
            {

            }
            return PBPDataTable;
        }


        public string GetJson(int forminstanceid)
        {
            string JsonData = string.Empty;
            return JsonData = this._folderVersionService.GetFormInstanceDataCompressed(1, forminstanceid);
        }

        public Dictionary<string, int> GetFromInstanceId(string qID)
        {
            Dictionary<string, int> Dis = new Dictionary<string, int>();
            try
            {
                //get ODM executed frominstanceid
                model.MigrationPlans BLforminstaneDetail =this._unitOfWork.RepositoryAsync<model.MigrationPlans>()
                                                          .Get().ToList()
                                                          .Where(s => s.QID.Equals(qID)
                                                          && s.ViewType.Equals("PBP")
                                                          )
                                                          .LastOrDefault();

                //get inporgress folderversionid 

                var InporgressFolderDetails = this._unitOfWork.RepositoryAsync<model.FolderVersion>().Get().ToList()
                                              .Where(s => s.FolderID.Equals(BLforminstaneDetail.FolderId)
                                              && s.FolderVersionStateID.Equals(1)
                                              )
                                              .FirstOrDefault();

                //get inprogress forminstaceid




                var IPFromInstacneDetail = this._unitOfWork.RepositoryAsync<model.FormInstance>().Get().ToList()
                                            .Where(s => s.FolderVersionID.Equals(InporgressFolderDetails.FolderVersionID)
                                            &&
                                            s.Name.Equals(qID)
                                            &&
                                            s.FormDesignVersionID.Equals(BLforminstaneDetail.FormDesignVersionId)
                                            )
                                          .FirstOrDefault();


                if (BLforminstaneDetail != null)
                {
                    Dis.Add("BL", BLforminstaneDetail.FormInstanceId);
                }
                if (IPFromInstacneDetail != null)
                {
                    Dis.Add("IP", IPFromInstacneDetail.FormInstanceID);
                }
            }
            catch (Exception ex)
            {
            }
            return Dis;
        }

        public List<DifferValueViewModel> ProcessHelper()
        {
            DataTable dt = this.GetQIDs();
            string baseLineFormInstanceData = string.Empty;
            string inPorgressFormInstanceData = string.Empty;
            Dictionary<string, int> Dict = null;
            JObject Source = null, target = null;
            IList<MigrationFieldItem> MigrationFieldList = GetBenefitMapping();
            List<DifferValueViewModel> ValueList = new List<DifferValueViewModel>();
            try
            {
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        Dict = new Dictionary<string, int>();
                        Dict= GetFromInstanceId(row["QID"].ToString());
                        
                        if (Dict.Count()>0)
                        {
                            baseLineFormInstanceData = GetJson(Dict["BL"]);
                            inPorgressFormInstanceData = GetJson(Dict["IP"]);
                        }
                        
                        if (!string.IsNullOrEmpty(baseLineFormInstanceData) && !string.IsNullOrEmpty(inPorgressFormInstanceData))
                        {
                            Source = JObject.Parse(baseLineFormInstanceData);
                            target = JObject.Parse(inPorgressFormInstanceData);

                            foreach (var item in MigrationFieldList)
                            {
                                string baselineValue = string.Empty;
                                string inPorgressValue = string.Empty;
                                if (Source.SelectToken(item.DocumentPath)!=null)
                                {
                                    baselineValue = Source.SelectToken(item.DocumentPath).ToString();
                                }

                                if (target.SelectToken(item.DocumentPath)!=null)
                                {
                                    inPorgressValue = target.SelectToken(item.DocumentPath).ToString();
                                }
                                
                                ValueList.Add
                                    (
                                    new DifferValueViewModel
                                    {
                                        QID = row["QID"].ToString(),
                                        DocumentPath = item.DocumentPath,
                                        BaselineValue = baselineValue,
                                        inPorgressValue = inPorgressValue,
                                        FieldName=  item.ColumnName.ToUpper(),
                                        TableName=item.TableName,
                                        BaseLineFormInstanceId= Dict["BL"],
                                        inProgressFormInstanceId= Dict["IP"]
                                    }
                                    );
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return ValueList;
        }

        private IList<MigrationFieldItem> GetBenefitMapping()
        {
            IList<MigrationFieldItem> migrationMapList = (from map in this._unitOfWork.RepositoryAsync<model.BenefitMapping>()
                       .Get()
                       .Where(a => a.ViewType == "PBP")
                                                          select new MigrationFieldItem
                                                          {
                                                              DocumentPath = map.DocumentPath,
                                                              Title = map.Title,
                                                              FieldTitle = map.FieldTitle,
                                                              ColumnName = map.ColumnName,
                                                              TableName = map.TableName
                                                          }).ToList();

            return migrationMapList;
        }
    }
}
