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
                MasterListMigration masterListtMigrator = new MasterListMigration();
                var res = masterListtMigrator.ExecuteMasterListMigration();
                if (res)
                {
                    Console.WriteLine("Completed!");
                }
                else
                {
                    Console.WriteLine("Error Occured.");
                }
                Console.ReadLine();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
