using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using tmg.equinox.applicationservices;
using tmg.equinox.dependencyresolution;
using tmg.equinox.repository.interfaces;
using tmg.equinox.schema.Base.Common;
using tmg.equinox.schema.Base.Interface;
using tmg.equinox.schema.Base.Model;
using tmg.equinox.schema.sql;

namespace tmg.equinox.schema.sql.Test
{
    [TestClass]
    public class SchemaTest
    {

        public SchemaTest()
        {
            UnityConfig.RegisterComponents();
        }
        [TestMethod]
        public void PrepareSchemaTest()
        {
            //var sqlSchema = new SQLSchema();
            //var success = sqlSchema.Run(new JsonDesign { jsonDesign = File.ReadAllText(@Config.JsonTemplatePath) },
            //    UnityConfig.Resolve<ISchemaRepository>());
            //Assert.IsTrue(success);

            var service = new GenerateSchemaService(UnityConfig.Resolve<IUnitOfWorkAsync> ());
            var success = service.Run(new JsonDesign { jsonDesign = File.ReadAllText(@Config.JsonTemplatePath) });
            Assert.IsTrue(success);
        }
    }
}
