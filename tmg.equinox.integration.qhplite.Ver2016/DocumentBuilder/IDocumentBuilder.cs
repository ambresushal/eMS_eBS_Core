using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tmg.equinox.integration.qhplite.Ver2016.DocumentBuilder
{
    public interface IDocumentBuilder
    {
        IList<QhpDocumentViewModel> BuildDocuments(string qhpFile, string defaultJSON);
    }
}
