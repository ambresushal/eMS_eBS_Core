using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.document.rulebuilder.model;

namespace tmg.equinox.document.rulebuilder.Validation
{
    public class RulesDependencyValidator
    {
        List<CompiledDocumentRule> _compiledDocuments;
        List<string> _ruleTargets;
        List<string> _sortedRules;

        public RulesDependencyValidator(List<CompiledDocumentRule> compiledRules,List<string> ruleTargets)
        {
            _compiledDocuments = compiledRules;
            _ruleTargets = ruleTargets;
        }

        public void ValidateDependencySequenceRules()
        {
            Dictionary<string,bool> visited=new Dictionary<string,bool>();
            foreach (var documentRule in _compiledDocuments)
            {
                CheckDependencyAndSortRule(documentRule, visited, _sortedRules);
            }
        }

        private void CheckDependencyAndSortRule(CompiledDocumentRule documentRule,Dictionary<string,bool> visited,List<string> _sortedRules)
        {

            //bool inProcess;
            //var alreadyVisited = visited.TryGetValue(documentRule.RuleAliace, out inProcess);

            //if (alreadyVisited)
            //{
            //    if (inProcess)
            //    {
            //        throw new ArgumentException("Cyclic dependency found.");
            //    }
            //}

            //else
            //{
            //    visited[documentRule.RuleAliace] = true;
                
            //    var derivedSources = documentRule.Source.RuleSources.Select(x => x).Where(whr => whr.SourceTargetMapType == SourceTargetMapType.Derived).Select(sel=>sel.SourceName);
            //    var dependentRules = from compiledRule in _compiledDocuments
            //                         where derivedSources.Contains(compiledRule.Target.TargetPath)
            //                         && !_sortedRules.Contains(documentRule.RuleAliace)
            //                         select compiledRule;

            //    foreach (var targetRule in dependentRules)
            //    {
            //        CheckDependencyAndSortRule(targetRule, visited, _sortedRules);
            //    }

            //    visited[documentRule.RuleAliace] = false;
            //    _sortedRules.Add(documentRule.RuleAliace);
            //}
        }
    }
}
