using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tmg.equinox.ruleinterpreter.model;
using Newtonsoft.Json;

namespace tmg.equinox.web.FormInstanceProcessor.ExpressionBuilder
{
    public class RuleTree
    {
        public List<DocumentRuleData> _compiledDocuments { get; set; }

        public List<int> trackCyclicDependency = new List<int>();

        public RuleTree(List<DocumentRuleData> documentData)
        {
            _compiledDocuments = documentData;
        }

        public string  RuleTreeFormulation()
        {
            RuleTreeCompiler compiledRuleTree = new RuleTreeCompiler();
            compiledRuleTree.rule = GetAllParentRuleList();
            return SerializedRuleTree(compiledRuleTree);
        }

        public List<ParentRule> GetAllParentRuleList()
        {
            List<ParentRule> items = new List<ParentRule>();

            foreach (var documentRule in _compiledDocuments)
            {
                ParentRule p = new ParentRule();
                p.id = documentRule.DocumentId.ToString();
                p.type = documentRule.DocumentType;

                var storeTarget = documentRule.CompileRule.Target.TargetPath;
                var docId = documentRule.DocumentId;

                trackCyclicDependency.Add(documentRule.DocumentId);
                p.rules = GetChildRuleList(docId,storeTarget);
                items.Add(p);
                trackCyclicDependency.Clear();
            }
            return items;
        }

        public List<ChildRules> GetChildRuleList( int parentDocId, string target)
        {
            var extractSource = (from m in _compiledDocuments
                                                        where m.DocumentId != parentDocId
                                                        select new
                                                        {
                                                           ID    =  m.DocumentId,
                                                           Rtype = m.DocumentType,
                                                           Target = m.CompileRule.Target.TargetPath,
                                                           Crule = m.CompileRule.SourceContainer.RuleSources.Where(i => i.SourcePath == target).Select(sel => sel.SourcePath).ToList()
                                                        }).Where(i => i.Crule != null && i.Crule.Count > 0)
                                                        .ToList();


            List<ChildRules> items = new List<ChildRules>();

            if (extractSource != null && extractSource.Count() > 0)
            {
                foreach (var item in extractSource)
                {
                    var exists = trackCyclicDependency.Contains(item.ID);
                    if (exists)
                    {
                        int Rule1 = item.ID;
                        int Rule2 = trackCyclicDependency[trackCyclicDependency.Count - 1];

                        var getTargetPath = (from m in _compiledDocuments
                                             where (m.DocumentId == Rule1 || m.DocumentId == Rule2)
                                             select new
                                             {
                                                 Target = m.CompileRule.Target.TargetPath,
                                             }).ToList();

                        var msg = getTargetPath[0].Target + "\n and \n" + getTargetPath[1].Target;

                        throw new ArgumentException("Cyclic Dependency found between: \n" + msg);
                    }
                    else
                    {

                        ChildRules c = new ChildRules();
                        c.id = item.ID.ToString();
                        c.type = item.Rtype;

                        trackCyclicDependency.Add(item.ID);
                        c.rules = GetChildRuleList(item.ID, item.Target);
                        trackCyclicDependency.RemoveAt(trackCyclicDependency.Count - 1);
                        items.Add(c);
                    }
                }
            }
            
            return items;
        }


        public string SerializedRuleTree(RuleTreeCompiler compiledRuleTree)
        {
            string serializedJSON = "";
            serializedJSON = JsonConvert.SerializeObject(compiledRuleTree);
            return serializedJSON;
        }

        public static CompiledDocumentRule Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<RootObject>(json).compileDocRule;
        }
    }
}