using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.expressionrules.tester
{
    public class ExcelDbContext
    {
        public static DataSet GetSheet(string filePath)
        {
            DataSet ds = new DataSet();

            OleDbConnection _connection = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=0\"");
            _connection.Open();

            DataTable sheets = _connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
            foreach (DataRow schemaRow in sheets.Rows)
            {
                string sheet = schemaRow["TABLE_NAME"].ToString();
                string query = "SELECT  * FROM [" + sheet + "]";
                OleDbDataAdapter daexcel = new OleDbDataAdapter(query, _connection);
                ds.Locale = CultureInfo.CurrentCulture;
                ds.Tables.Add(sheet);
                daexcel.Fill(ds.Tables[sheet]);
            }

            _connection.Close();

            return ds;
        }

    }
}
