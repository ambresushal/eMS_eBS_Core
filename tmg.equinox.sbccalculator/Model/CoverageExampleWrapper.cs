using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.sbccalculator.Model
{
   public class CoverageExampleWrapper
    {
        public List<CoverageExample> FractureCoverageExample { get; set; }
        public List<CoverageExample> MaternityCoverageExample { get; set; }
        public List<CoverageExample> DiabetesCoverageExample { get; set; }
        public CoverageExampleWrapper()
        {
            this.DiabetesCoverageExample = new List<CoverageExample>();
            this.MaternityCoverageExample = new List<CoverageExample>();
            this.DiabetesCoverageExample = new List<CoverageExample>();
        }
    }
}
