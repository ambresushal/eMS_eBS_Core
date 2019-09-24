using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace tmg.equinox.integration.qhplite.Ver2016
{
    public class ExcelFileReader : IExcelFileReader 
    {
        public MemoryStream GetExcelFile(string fileName)
        {
            MemoryStream qhpStream = new MemoryStream();
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read)) 
            {
                qhpStream.SetLength(fs.Length);
                //read file to MemoryStream
                fs.Read(qhpStream.GetBuffer(), 0, (int)fs.Length);
            }
            return qhpStream;
        }
    }
}
