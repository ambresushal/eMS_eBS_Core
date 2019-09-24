using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.config;
using tmg.equinox.core.logging.Logging;
using tmg.equinox.schema.Base.Common;
using tmg.equinox.schema.Base.Interface;
using tmg.equinox.schema.Base.Model;

namespace tmg.equinox.schema.Base
{
    public class JSonSchema : IJSchema
    {
        JObject jsonSchema = new JObject();
        private static readonly ILog _logger = LogProvider.For<JSonSchema>();
        public JSonSchema()
        {
            LoadTemplate();
        }
        public JSonSchema(JsonDesign jsonDesign)
        {
            LoadTemplate(jsonDesign);
        }
        private void LoadTemplate(JsonDesign jsonDesign)
        {
            jsonSchema = JObject.Parse(jsonDesign.JsonDesignData);
            _logger.Debug("JSON Design template loaded.");
        }
        private void LoadTemplate()
        {
            jsonSchema = JObject.Parse(File.ReadAllText(@Config.JsonTemplatePath));
        }

        public JToken Get()
        {
            _logger.Debug("Get Sections object from JSON Design.");
            return jsonSchema["Sections"];
        }
    }
}
