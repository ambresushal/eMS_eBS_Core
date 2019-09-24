using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.domain.services
{
    public class FileWritter
    {
        public void CreateFile(string filePath,string fileName, string content)
        {
            bool exists = System.IO.Directory.Exists(filePath);

            if (!exists)
                System.IO.Directory.CreateDirectory(filePath);
            string path = filePath + "\\" + fileName;
            if (!File.Exists(path))
            {
                File.Create(path).Close();
            }
            File.AppendAllText(path, content);  
        }
    }
}
