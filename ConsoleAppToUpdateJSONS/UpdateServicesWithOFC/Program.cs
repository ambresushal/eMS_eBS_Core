using System;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Xml;


namespace UpdateServicesWithOVC
{
    class Program
    {       
        static void Main(string[] args)
        {

            string s = string.Empty;

            UpdateServices.Update_Product();

            string Result = UpdateServices.Result.ToString();
            Result = Result.Replace("\n", Environment.NewLine);
            string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            path = path + "\\Result_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".txt";
            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter wr = new StreamWriter(fs))
                {
                    wr.WriteLine(Result);
                }
            }
        }
    }
}






