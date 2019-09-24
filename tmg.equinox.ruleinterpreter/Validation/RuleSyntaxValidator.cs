using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.ruleinterpreter.validation
{
    public class RuleSyntaxValidator
    {
        public bool IsParsable<U>(U value, string T, ref IList<string> jsonParsingErrorMessages) where U : class
        {
            JsonSchemaGenerator generator = new JsonSchemaGenerator();
            JsonSchema schema = generator.Generate(typeof(U));
            JObject person = JObject.Parse(T);
            bool valid = person.IsValid(schema, out jsonParsingErrorMessages);

            return valid;
        }
    }
}
