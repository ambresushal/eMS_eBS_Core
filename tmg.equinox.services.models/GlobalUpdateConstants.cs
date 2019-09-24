using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels
{
    public class GlobalUpdateConstants
    {
        public static string[] trueList = { "true", "True", "Yes", "yes", "Selected" };
        public static string[] falseList = { "false", "False", "No", "no", "Not Selected" };
        public static string[] equalsList = { null, "", "undefined", "None", "Choose" };

        public const string NA = "NA";
        public const string NULL = "NULL";
        public const string CHOOSE = "Choose";
        public const string NONE = "None";
        public const string SELECTED = "Selected";
        public const string NOTSELECTED = "Not Selected";
        public const string YES = "Yes";
        public const string NO = "No";

        public const string IASFolderPath = "\\App_Data\\IAS\\";
        public const string IASErrorLogFolderPath = "\\App_Data\\IAS\\ErrorLog\\";
        public const string IASReportText = "Global Update Report - ";
        public const string ErrorLogReportText = "Error Log Report - ";
        public const string ExcelFileExtension = ".xlsx";
        public const string ExcelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        public const string PasswordforExcelSheet = "gusuperuser";
        public const string IASReportSheetName = "Impact Assessment";
        public const string ErrorLogReportSheetName = "Error Log";
        public const string ExcelErrorTitle = "Incorrect Option";
        public const string ExcelError = "You must select an option from the list.";
    }
}
