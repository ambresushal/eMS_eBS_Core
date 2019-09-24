using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace tmg.equinox.integration.qhplite.Ver2016
{
    interface IExcelFileReader
    {
        MemoryStream GetExcelFile(string fileName);
    }
}
