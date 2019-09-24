using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using tmg.equinox.ruleinterpreter.model;

namespace tmg.equinox.web.sourcehandler
{
    public class SourceGroupingWrapper
    {
        Dictionary<string, string> _sources;
        Dictionary<string, int> _ruleAliasFormInstances;

        public SourceGroupingWrapper(Dictionary<string, string> sources, Dictionary<string, int> ruleAliasFormInstances)
        {
            _sources = sources;
            _ruleAliasFormInstances = ruleAliasFormInstances;
        }

        public List<SourceHandlerInput> GetSourceHandlerInput()
        {
            List<SourceHandlerInput> sourceHandlerInputs = new List<SourceHandlerInput>();
            IEnumerable<IGrouping<string, SourceElement>> ruleAliasGroups = GetGroupedRuleAliases();

            foreach (var ruleAliasGroup in ruleAliasGroups)
            {
                SourceHandlerInput sourceHandlerInput = new SourceHandlerInput();
                List<SourceSection> sourceSections = new List<SourceSection>();
                IEnumerable<IGrouping<string, SourceElement>> sectionGroups = ruleAliasGroup.GroupBy(grp => grp.Section).Select(sel => sel).ToList();
                sourceHandlerInput.RuleAlias = ruleAliasGroup.Key;
                sourceHandlerInput.FormInstanceId = _ruleAliasFormInstances[ruleAliasGroup.Key];

                foreach (var sectionGroup in sectionGroups)
                {
                    SourceSection sourceSection = new SourceSection();
                    sourceSection.SectionName = sectionGroup.Key;
                    sourceSection.SourceInput = GetSourceInputs(sectionGroup);
                    sourceSections.Add(sourceSection);
                }
                sourceHandlerInput.SourceSection = sourceSections;
                sourceHandlerInputs.Add(sourceHandlerInput);
            }

            return sourceHandlerInputs;
        }
        private List<SourceInput> GetSourceInputs(IGrouping<string, SourceElement> sectionGroup)
        {
            return sectionGroup.Select(c =>
                              new SourceInput
                              {
                                  SourceName = c.SourceName,
                                  SectionElementPaths=c.SectionElementPaths
                              }).ToList();
        }
        private IEnumerable<IGrouping<string, SourceElement>> GetGroupedRuleAliases()
        {
            IEnumerable<IGrouping<string, SourceElement>> groupedRuleAliases = (from source in _sources
                                                                               select new SourceElement
                                                                               {
                                                                                   RuleAlias = source.Value.Substring(0, source.Value.IndexOf('[')),
                                                                                   Section = source.Value.Contains('.') ? source.Value.Substring(source.Value.IndexOf('[') + 1, (source.Value.IndexOf('.') - source.Value.IndexOf('[')) - 1) : source.Value.Substring(source.Value.IndexOf('[') + 1, (source.Value.IndexOf(']') - source.Value.IndexOf('[')) - 1),
                                                                                   SourceName = source.Key,
                                                                                   SectionElementPaths = GetSourceElements(source.Value)
                                                                               }).GroupBy(m => m.RuleAlias);

            return groupedRuleAliases;
        }


        private List<string> GetSourceElements(string sourcePath)
        {
            List<string> sourceElements = new List<string>();
            string regularExpressionPattern = @"(?<=\[)(.*?)(?=\])";
            Regex regex = new Regex(regularExpressionPattern);
            foreach (Match m in regex.Matches(sourcePath))
            {
                sourceElements.Add(m.Value);
            }
            return sourceElements;
        }
    }
}
