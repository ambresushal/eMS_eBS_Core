using System;
using tmg.equinox.dependencyresolution;

namespace tmg.equinox.integration.migration
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                UnityConfig.RegisterComponents();

                Console.WriteLine("Running...");
                ProductMigrator productMigrator = new ProductMigrator();
                var res = productMigrator.ExecuteProcess();
                if (res)
                {
                    Console.WriteLine("Completed!");
                }
                else
                {
                    Console.WriteLine("Error Occured.");
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
