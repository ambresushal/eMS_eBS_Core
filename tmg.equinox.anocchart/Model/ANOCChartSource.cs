using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.anocchart.Model
{
    public class ANOCChartSource
    {

      public JObject PreviousMedicareJsonData { get; set; }
      public JObject NextMedicareJsonData { get; set; }
      public JObject PreviousPBPViewJsonData { get; set; }
      public JObject NextPBPViewJsonData { get; set; }
      public JObject MasterListAnocJsonData { get; set; }
      public JObject AnocViewJsonData { get; set; }
      public JObject MasterListANOCEOCJsonData { get; set; }
      public AnocchartHelper AnocHelper { get; set; }

    }
}
