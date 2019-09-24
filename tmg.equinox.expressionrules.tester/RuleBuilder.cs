using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.document.rulebuilder.model;


namespace tmg.equinox.expressionrules.tester
{
    public class RuleBuilder
    {
        string _filePath = string.Empty;
        public RuleBuilder(string path)
        {
            //_filePath = @"D:\PROJECTS\Equinox\NET\HNE\DEV\tmg.equinox.expressionrules.tester\Files\HNE_EOC_RulesDefination.xlsx";
            _filePath = path;
        }

        public List<RuleDefinition> Build(int formDesignID, int formDesignVersionID)
        {
            List<RuleDefinition> rules = new List<RuleDefinition>();

            DataSet ds = ExcelDbContext.GetSheet(_filePath);

            foreach (DataTable dt in ds.Tables)
            {
                var result = from rule in dt.AsEnumerable() group rule by rule["TargetUIELementID"] into newGroup select newGroup;
                foreach (var nameGroup in result)
                {
                    if (!string.IsNullOrEmpty(Convert.ToString(nameGroup.Key)))
                    {
                        var objTarget = nameGroup.First();

                        Documentrule objRule = new Documentrule();
                        objRule.targetelement = Convert.ToString(objTarget["TargetRuleAlias"]).Trim() + "[" + Convert.ToString(objTarget["TargetElementPath"]).Trim() + "]";
                        //objRule.ruletype = "datasource";
                        //objRule.targetelementtype = "field";

                        char sourceName = 'A';
                        RuleConditions ruleCondition = new RuleConditions();
                        List<Source> sources = new List<Source>();

                        var sourceList = from source in nameGroup group source by source["SourceElementPath"] into sourceGroup select sourceGroup;
                        foreach (var grp in sourceList)
                        {
                            var s = grp.First();
                            //Build a source
                            Source source = new Source();
                            source.sourcename = sourceName.ToString();
                            source.sourceelement = Convert.ToString(s["SourceRuleAlias"]).Trim() + "[" + Convert.ToString(s["SourceElementPath"]).Trim() + "]";
                            //source.filter = new Filter() { filterlist = new List<Filterlist>(), filtermergeexpression = "", filtermergetype = "none", keycolumns = "" };
                            //source.outputcolumns = null;
                            sourceName = (Char)(Convert.ToUInt16(sourceName) + 1); ;
                            sources.Add(source);
                        }
                        ruleCondition.sources = sources;
                        ruleCondition.sourcemergelist = GetCustomScript(Convert.ToString(objTarget["SourceElementKey"]), Convert.ToString(objTarget["SourceElementColumn"]));
                        objRule.ruleconditions = ruleCondition;

                        DocumentRuleContainer container = new DocumentRuleContainer();
                        container.documentrule = objRule;
                        string ruleJSON = JsonConvert.SerializeObject(container);

                        rules.Add(new RuleDefinition()
                        {
                            DisplayText = "Populate " + objRule.targetelement,
                            Description = "Populate " + objRule.targetelement,
                            RuleJSON = ruleJSON,
                            FormDesignID = formDesignID,
                            FormDesignVersionID = formDesignVersionID,
                            TargetUIElementID = Convert.ToInt32(objTarget["TargetUIELementID"]),
                            TargetElementPath = objRule.targetelement
                        });
                    }
                }
            }
            return rules;
        }

        public Sourcemergelist GetCustomScript(string key, string column)
        {
            Sourcemergelist sourcemergelist = new Sourcemergelist();
            //sourcemergelist.outputcolumns = new Outputcolumns();
            sourcemergelist.sourcemergeactions = new List<Sourcemergeaction>() { 
                new Sourcemergeaction(){
                    sourcemergetype = "script",
                    sourcemergeexpression = ScriptTemplate(key,column),
                    //keycolumns = "",
                    //mappings = new Mappings(){sourcefields="",targetfields=""}
                }
            };

            return sourcemergelist;
        }

        public string ScriptTemplate(string key, string column)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SET(langFilter, \"{'Logicaloperator' : 'AND' , 'Expression' : [{'Column' : 'Key', 'Operand' : '=', 'Value': '" + key + "'}]}\");");
            sb.Append("SET(result, FILTER(a, langFilter, \"" + column + "\"));");
            sb.Append("SETTEXT(target, result);");
            return sb.ToString();
        }
    }
}
