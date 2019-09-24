using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.ruleinterpreter.globalUtility;
using tmg.equinox.ruleinterpreter.model;
using tmg.equinox.ruleinterpreter.jsonhelper;

namespace tmg.equinox.ruleinterpreter.rulecompiler
{
    public class OutputPropertyCompiler
    {
        Outputcolumns _outputColumns;

        public OutputPropertyCompiler(Outputcolumns outputColumns)
        {
            _outputColumns = outputColumns;
        }

        public OutputProperties ConvertOutputColumnToProperties()
        {
            OutputProperties _outputProperties = new OutputProperties();
            _outputProperties.Columns = new Dictionary<string, List<KeyValuePair<string, string>>>();
            _outputProperties.Children = new Newtonsoft.Json.Linq.JObject();

            _outputProperties.Columns = !string.IsNullOrEmpty(_outputColumns.columns) ? RuleEngineGlobalUtility.GetColumnMappings(_outputColumns.columns) : _outputProperties.Columns;
            _outputProperties.Children = (_outputColumns.children !=null &&_outputColumns.children.Count > 0 ) ? _outputColumns.children.ConvertChildColumnToJproperties() : _outputProperties.Children;
            return _outputProperties;
        }
    }
}
