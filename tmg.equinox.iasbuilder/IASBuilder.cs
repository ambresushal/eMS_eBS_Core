using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.GlobalUpdate;

namespace tmg.equinox.iasbuilder
{
    public class IASBuilder
    {
        #region Private Memebers



        #endregion Private Members

        #region Public Properties

        public static int IASFolderMapHeaderRow = 1;
        public static int IASFolderMapValueRow = 2;
        public static int IASElementExportHeaderRow = 3;
        public static int IASElementExportValueRow = 4;
        public static int ErrorLogHeaderRow = 5;
        public static int ErrorLogValueRow = 6;

        #endregion Public Properties

        #region Constructor

        public IASBuilder()
        {

        }

        #endregion Constructor

        #region Public Methods

        public List<object> BuildConfig(int GlobalUpdateID, List<IASFolderDataModel> DocumentInfo, List<IASElementExportDataModel> ElementUpdateInfo, List<FormDesignElementValueVeiwModel> ElementSelectionInfo)
        {
            List<object> config = new List<object>();
            config.Add(GlobalUpdateID);
            config.Add(DocumentInfo);
            config.Add(ElementUpdateInfo);
            config.Add(ElementSelectionInfo);
            return config;
        }

        public List<DocumentInstanceModel> BuildDocumentInstance(int GlobalUpdateID, List<object> Config, List<FormDesignElementValueVeiwModel> designdocumentsInfo)
        {
            List<DocumentInstanceModel> instance = new List<DocumentInstanceModel>();
            foreach (var document in designdocumentsInfo)
            {
                DocumentInstanceModel docInst = new DocumentInstanceModel();
                docInst.FormDesignID = document.FormDesignID;
                docInst.DocumentName = document.FormDesignName;
                docInst.csv = BuildIASRow(GlobalUpdateID, Config, Convert.ToInt32(document.FormDesignID));
                instance.Add(docInst);
            }

            return instance;
        }

        public string BuildIASRow(int GlobalUpdateID, List<object> Config, int FormDesignID)
        {
            IList<IASFolderDataModel> iasFolderDataListO = Config[1] as IList<IASFolderDataModel>;
            List<IASFolderDataModel> iasFolderDataList = iasFolderDataListO.Where(xy => xy.FormDesignID == FormDesignID).ToList();
            IList<IASElementExportDataModel> iasElementExportDataListO = Config[2] as IList<IASElementExportDataModel>;
            List<IASElementExportDataModel> iasElementExportDataList = iasElementExportDataListO.Where(xy => xy.FormDesignID == FormDesignID).ToList();
            //List<IASElementExportDataModel> iasElementSelectionList = iasElementExportDataList.Where(xy => xy.FormDesignID == FormDesignID).GroupBy(xy => xy.UIElementID).Select(xy => xy.FirstOrDefault()).ToList();
            IList<FormDesignElementValueVeiwModel> iasElementSelectionListO = Config[3] as IList<FormDesignElementValueVeiwModel>;
            List<FormDesignElementValueVeiwModel> iasElementSelectionList = iasElementSelectionListO.Where(xy => xy.FormDesignID == FormDesignID).ToList();
            
            string csv = string.Empty;

            //csv = ToCsvHeader("\t", iasFolderDataList, IASFolderMapHeaderRow);
            csv = "GlobalUpdateID\tAccount or Portfolio\tFolderID\tFolder Name\tFolderVersionID\tFolder Version No.\tFolder Effective Date\tFormInstanceID\tDocument Name\tOwner";

            foreach (var iasElementSelection in iasElementSelectionList)
            {
                csv += "\t\t\t\t";
                csv += ToCsvRow("\t", iasElementSelectionList, iasElementSelection, IASElementExportHeaderRow);
                csv += "\t";
            }

            csv += "\r\ndummyColumn\tdummyColumn\tdummyColumn\tdummyColumn\tdummyColumn\tdummyColumn\tdummyColumn\tdummyColumn\tdummyColumn\tdummyColumn\t";
            foreach (var iasElementSelection in iasElementSelectionList)
            {
                csv += "Current Value\tOptionLabel\tOptionLabelNo\tItemData\tNew Value\tAccept Change\t";
            }
            csv += "\r\n";

            foreach (var iasFolderData in iasFolderDataList)
            {
                csv += ToCsvRow("\t", iasFolderDataList, iasFolderData, IASFolderMapValueRow);

                foreach (var iasElementSelection in iasElementSelectionList)
                {
                    IASElementExportDataModel iasElementExportDataValues = GetIASElementExportData(iasElementExportDataList, iasFolderData.IASFolderMapID, iasFolderData.FormInstanceID, iasElementSelection.Label);
                    csv += "\t";
                    if (iasElementExportDataValues != null)
                    {
                        if (iasElementExportDataValues.OldValue.Contains("\n"))
                        {
                            csv += iasElementExportDataValues.OldValue.Replace("\n", "");
                        }
                        else if (GlobalUpdateConstants.equalsList.Contains(iasElementExportDataValues.OldValue))
                        {
                            csv += GlobalUpdateConstants.NULL;
                        }
                        else
                        {
                            csv += iasElementExportDataValues.OldValue;
                        }
                        csv += "\t";
                        if (iasElementExportDataValues.OptionLabel != null)
                        {
                            csv += iasElementExportDataValues.OptionLabel;
                        }
                        else
                        {
                            csv += GlobalUpdateConstants.NULL;
                        }
                        csv += "\t";
                        if (iasElementExportDataValues.OptionLabelNo != null)
                        {
                            csv += iasElementExportDataValues.OptionLabelNo;
                        }
                        else
                        {
                            csv += GlobalUpdateConstants.NULL;
                        }
                        csv += "\t";
                        if (iasElementSelection.ItemData != null)
                        {
                            csv += iasElementSelection.ItemData;
                        }
                        else
                        {
                            csv += GlobalUpdateConstants.NULL;
                        }
                        csv += "\t";
                    }
                    //csv += ToCsvRow("\t", iasElementSelectionList, iasElementSelection, IASElementExportValueRow);
                    if (iasElementSelection.NewValue != null)
                    {
                        if (iasElementSelection.NewValue.Contains("\n"))
                        {
                            csv += iasElementSelection.NewValue.Replace("\n", "");
                        }
                        else if (GlobalUpdateConstants.trueList.Contains(iasElementSelection.NewValue))
                        {
                            if (iasElementSelection.UIElementTypeID == ElementTypes.RADIOID)
                            {
                                csv += iasElementSelection.OptionLabel;
                            }
                            else if (iasElementSelection.UIElementTypeID == ElementTypes.CHECKBOXID)
                            {
                                csv += GlobalUpdateConstants.SELECTED;
                            }
                            else
                            {
                                csv += iasElementSelection.NewValue;
                            }
                        }
                        else if (GlobalUpdateConstants.falseList.Contains(iasElementSelection.NewValue))
                        {
                            if (iasElementSelection.UIElementTypeID == ElementTypes.RADIOID)
                            {
                                csv += iasElementSelection.OptionLabelNo;
                            }
                            else if (iasElementSelection.UIElementTypeID == ElementTypes.CHECKBOXID)
                            {
                                csv += GlobalUpdateConstants.NOTSELECTED;
                            }
                            else
                            {
                                csv += iasElementSelection.NewValue;
                            }
                        }
                        else if (GlobalUpdateConstants.equalsList.Contains(iasElementSelection.NewValue))
                        {
                            csv += GlobalUpdateConstants.NULL;
                        }
                        else if (iasElementSelection.NewValue == GlobalUpdateConstants.NA)
                        {
                            if (iasElementSelection.UIElementTypeID == ElementTypes.RADIOID)
                            {
                                csv += GlobalUpdateConstants.CHOOSE;
                            }
                            else if (iasElementSelection.UIElementTypeID == ElementTypes.CHECKBOXID)
                            {
                                csv += GlobalUpdateConstants.NONE;
                            }
                            else
                            {
                                csv += iasElementSelection.NewValue;
                            }
                        }
                        else
                        {
                            csv += iasElementSelection.NewValue;
                        }
                    }
                    else
                    {
                        if (iasElementSelection.UIElementTypeID == ElementTypes.RADIOID)
                        {
                            csv += GlobalUpdateConstants.CHOOSE;
                        }
                        else if (iasElementSelection.UIElementTypeID == ElementTypes.CHECKBOXID)
                        {
                            csv += GlobalUpdateConstants.NONE;
                        }
                        else
                        {
                            csv += GlobalUpdateConstants.NA;
                        }
                    }
                    csv += "\t";
                    if (iasElementExportDataValues != null)
                    {
                        if (iasElementExportDataValues.AcceptChange == true)
                        {
                            csv += GlobalUpdateConstants.YES;
                        }
                        else
                        {
                            csv += GlobalUpdateConstants.NO;
                        }
                    }
                }

                csv += "\r\n";
            }

            return csv;
        }

        public string BuildErrorLogRow(int GlobalUpdateID, List<ErrorLogViewModel> ErrorLogInfo)
        {
            string csv = string.Empty;

            //csv = ToCsvHeader("\t", ErrorLogInfo, ErrorLogHeaderRow);
            csv = "GlobalUpdateID\tIASElementExportID\tIASFolderMapID\tAccount Name\tFolderID\tFolder Name\tFolderVersionID\tFolder Version Number\tEffective Date\tFormInstanceID\tForm Name\tOwner\tElement Name\tElement FullPath\tRule Error Description\tDataSource Error Description\tValidation Error Description\tAdded By\tAdded Date";
            csv += "\r\n";
                        
            foreach (var errorData in ErrorLogInfo)
            {                                           
                csv += ToCsvRow("\t", ErrorLogInfo, errorData, ErrorLogValueRow);
                csv += "\r\n";                         
            }                                          
                                                       
            return csv;                                
        }

        #endregion Public Methods

        #region Private Methods

        private static string ToCsvHeader<T>(string separator, IEnumerable<T> objectlist, int flag)
        {
            Type t = typeof(T);
            PropertyInfo[] props = t.GetProperties();
            PropertyInfo[] filterprops = null;
            if (flag == IASFolderMapHeaderRow)
            {
                filterprops = props.Where(p => p.Name != "IASFolderMapID" && p.Name != "FormDesignID").ToArray();
            }
            else if (flag == ErrorLogHeaderRow)
            {
                filterprops = props.Where(p => p.Name != "ErrorLogID" && p.Name != "FormDesignID" && p.Name != "UpdatedBy" && p.Name != "UpdatedDate" && p.Name != "RoleClaim").ToArray();
            }

            string header = String.Join(separator, filterprops.Select(f => f.Name).ToArray());

            StringBuilder csvdata = new StringBuilder();
            csvdata.AppendLine(header);

            return Convert.ToString(csvdata);
        }

        private static string ToCsvRow<T>(string separator, IEnumerable<T> objectlist, object o, int flag)
        {
            Type t = typeof(T);
            PropertyInfo[] props = t.GetProperties();
            PropertyInfo[] filterprops = null;
            if (flag == IASFolderMapValueRow)
            {
                filterprops = props.Where(p => p.Name != "IASFolderMapID" && p.Name != "FormDesignID").ToArray();
            }
            else if (flag == IASElementExportHeaderRow)
            {
                filterprops = props.Where(p => p.Name != "IASElementExportID" && p.Name != "GlobalUpdateID" && p.Name != "UIElementID" &&
                                                        p.Name != "UIElementName" && p.Name != "IASFolderMapID" && p.Name != "FormInstanceID" &&
                                                        p.Name != "OldValue" && p.Name != "NewValue" && p.Name != "AcceptChange" &&
                                                        p.Name != "UIElementTypeID" && p.Name != "OptionLabel" && p.Name != "OptionLabelNo" &&
                                                        p.Name != "FormDesignID" && p.Name != "FormDesignElementValueID" && p.Name != "IsParentRepeater" &&
                                                        p.Name != "FormDesignVersionID" && p.Name != "ElementHeaderName" && p.Name != "IsUpdated" &&
                                                        p.Name != "IsValueUpdated" && p.Name != "FormDesignName" && p.Name != "Name" &&
                                                        p.Name != "Label" && p.Name != "AddedBy" && p.Name != "AddedDate" &&
                                                        p.Name != "UpdatedBy" && p.Name != "UpdatedDate" && p.Name != "RoleClaim").ToArray();
            }
            else if (flag == IASElementExportValueRow)
            {
                filterprops = props.Where(p => p.Name != "IASElementExportID" && p.Name != "GlobalUpdateID" && p.Name != "UIElementID" &&
                                                        p.Name != "UIElementName" && p.Name != "IASFolderMapID" && p.Name != "FormInstanceID" &&
                                                        p.Name != "OldValue" && p.Name != "NewValue" && p.Name != "AcceptChange" &&
                                                        p.Name != "UIElementTypeID" && p.Name != "OptionLabel" && p.Name != "OptionLabelNo" &&
                                                        p.Name != "ItemData" && p.Name != "FormDesignID" && p.Name != "IsParentRepeater").ToArray();
            }
            else if (flag == ErrorLogValueRow)
            {
                filterprops = props.Where(p => p.Name != "ErrorLogID" && p.Name != "FormDesignID" && p.Name != "UpdatedBy" && p.Name != "UpdatedDate" && p.Name != "RoleClaim").ToArray();
            }

            StringBuilder linie = new StringBuilder();

            foreach (var f in filterprops)
            {
                if (linie.Length > 0)
                    linie.Append(separator);

                var x = f.GetValue(o);

                if (x != null)
                {
                    if (Convert.ToString(x.GetType()) == "System.DateTime")
                    {
                        DateTime date = Convert.ToDateTime(Convert.ToString(x));
                        linie.Append(date.ToShortDateString());
                    }
                    else if (Convert.ToString(x).Contains("\n"))
                    {
                        linie.Append(Convert.ToString(x).Replace("\n", ""));
                    }
                    else
                    {
                        linie.Append(Convert.ToString(x));
                    }
                }
            }

            return Convert.ToString(linie);
        }

        private IASElementExportDataModel GetIASElementExportData(IList<IASElementExportDataModel> IASElementExportDataList, int IASFolderMapID, int formInstanceId, string label)
        {
            IASElementExportDataModel iasElementExportDataValues = null;

            var globalUpdatesDataValues = (from row in IASElementExportDataList
                                            where row.IASFolderMapID == IASFolderMapID && row.FormInstanceID == formInstanceId && row.Label == label
                                           select row).FirstOrDefault();
            
            iasElementExportDataValues = new IASElementExportDataModel
                                            {
                                                Label = globalUpdatesDataValues.Label,
                                                OldValue = globalUpdatesDataValues.OldValue,
                                                NewValue = globalUpdatesDataValues.NewValue,
                                                AcceptChange = globalUpdatesDataValues.AcceptChange,
                                                OptionLabel = globalUpdatesDataValues.OptionLabel,
                                                OptionLabelNo = globalUpdatesDataValues.OptionLabelNo,
                                                ItemData = globalUpdatesDataValues.ItemData
                                            };            

            return iasElementExportDataValues;
        }

        #endregion Private Methods

    }
}
