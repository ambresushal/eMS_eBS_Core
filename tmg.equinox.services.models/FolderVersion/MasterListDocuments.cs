using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.FolderVersion
{
    public class MasterListDocuments
    {
        public List<Section> Sections { get; set; }
    }

    public class Section
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public bool Visible { get; set; }
        public bool Enabled { get; set; }
    }
}
