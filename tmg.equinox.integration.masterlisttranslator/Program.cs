using System;
using tmg.equinox.dependencyresolution;

namespace tmg.equinox.integration.masterlisttranslator
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Running...");

                UnityConfig.RegisterComponents();

                FacetMasterListTranslator facetMasterListTranslator = new FacetMasterListTranslator();
                var res = facetMasterListTranslator.ExecuteMasterListTranslator();

                Console.WriteLine("Completed!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace.ToString());
            }
        }
    }
}
