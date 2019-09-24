using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.config;

namespace tmg.equinox.reporting.Base.SQLDataAccess
{


    public class DataHolder
    {
        public int index { get; set; }
        public ICollection<IDictionary<string, object>> Data { get; set; }
    }

    public class BaseDBHelper
    {
        private int ctr = 0;

        public ICollection<DataHolder> Get(string sqlStatment)
        {
            SqlConnection con = null;
            // Create and Open the SQL server connection object
            using (con = new SqlConnection(Config.GetReportingCenterConnectionString()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(sqlStatment, con);
                return Map(cmd);
            }
        }

        private IEnumerable<IDictionary<string, object>> GetResultSet(IDataReader reader)
        {
            int i = 0;
            var names = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList();
            foreach (IDataRecord record in reader as IEnumerable)
            {
                var expando = new ExpandoObject() as IDictionary<string, object>;
                foreach (var name in names)
                    expando[name] = record[name];

                yield return expando;
            }
        }

        private IEnumerable<IDictionary<string, object>> GetNextResultSet(IDataReader reader, ICollection<DataHolder> dataList, int ctr)
        {
            if (reader.NextResult())
            {
                var list = GetResultSet(reader);
                if (list == null)
                    return null;
                else
                {
                    dataList.Add(new DataHolder { index = dataList.Count(), Data = list.ToList() });
                    //Change made on this line
                    var nextResult = GetNextResultSet(reader, dataList, dataList.Count());
                    if (nextResult == null)
                        return null;
                }

            }
            return null;
        }

        private ICollection<DataHolder> Map(SqlCommand cmd)
        {
            cmd.CommandTimeout = 0;
            using (var reader = cmd.ExecuteReader())
            {
                ctr = 0;
                var result = GetResultSet(reader);
                var dataList = new List<DataHolder>();
                dataList.Add(new DataHolder { index = ctr, Data = result.ToList() });

                GetNextResultSet(reader, dataList, ctr);
                // if(reader!=null)
                //       dataList.Add(new DataHolder { index = ctr, Data = result.ToList() });
                ctr = 0;
                return dataList;
            }
        }

        public DataSet GetData(string sqlStatment)
        {
            SqlConnection con = null;
            // Create and Open the SQL server connection object
            using (con = new SqlConnection(Config.GetReportingCenterConnectionString()))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(sqlStatment, con);
                return MaptoDatable(cmd);
            }
        }


        private DataSet MaptoDatable(SqlCommand cmd)
        {
            DataSet ds = new DataSet();
            using (var reader = cmd.ExecuteReader())
            {
                int i = 0;
                ds.Tables[i].Load(reader);
                GetNextResultSet(reader, ds, i);
            }
            return ds;
        }

        private DataSet GetNextResultSet(IDataReader reader, DataSet ds, int ctr)
        {

            if (reader.NextResult())
            {
                while (reader.Read())
                {
                    ds.Tables[ctr].Load(reader);
                }
                GetNextResultSet(reader, ds, ctr++);
            }
            return ds;
        }

        private ICollection<DataHolder> ExecuteStoreProc(string sqlStatment, List<SqlParameter> lstParam)
        {
            SqlConnection con = null;
            SqlDataReader rd = null;
            // Create and Open the SQL server connection object
            using (con = new SqlConnection(Config.GetReportingCenterConnectionString()))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(sqlStatment, con);

                cmd.Parameters.AddRange(lstParam.ToArray());

                return Map(cmd);

            }
        }
    }
}
