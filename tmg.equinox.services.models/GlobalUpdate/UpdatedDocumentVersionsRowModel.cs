using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.GlobalUpdate
{
  public class UpdatedDocumentVersionsRowModel : ViewModelBase
    {
      public int GlobalUpdateId { get; set; }
      public string FormDesignName { get; set; }
      public int FormDesignId { get; set; }
      public int FormDesignVersionId { get; set; }
      public DateTime LastUpdatedOn { get; set; }
      public string Version { get; set; }
    }
}
