using System;
using tmg.equinox.dependencyresolution;

namespace tmg.equinox.integration.translator.translators
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Running...");                

                UnityConfig.RegisterComponents();
                
                Facet481Translator facet481Translator = new Facet481Translator();
                var res = facet481Translator.ExecuteTranslator();

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
