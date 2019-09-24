using System;
using tmg.equinox.dependencyresolution;

namespace tmg.equinox.integration.transmitter
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Running...");

                if (args.GetLength(0) > 0)
                {
                    string environment = args[0];
                    if (!string.IsNullOrEmpty(environment))
                    {
                        UnityConfig.RegisterComponents();

                        MasterListTransmitter facetTransmitter = new MasterListTransmitter();
                        var res = facetTransmitter.ExecuteMasterListTransmitter(environment.ToUpper());
                    }
                }

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
