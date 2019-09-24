using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using tmg.equinox.ruleinterpreter.model;

namespace tmg.equinox.expressionbuilder
{
    public class EventTree
    {
        public List<DocumentRuleData> _compiledDocuments { get; set; }
        public string formName { get; set; }
        public int formDesignVersionId { get; set; }

        public EventTree(List<DocumentRuleData> documentData)
        {
            _compiledDocuments = documentData;
        }

        public string EventTreeFormulation()
        {
            RuleEventCompiler compiledEventTree = new RuleEventCompiler();
            formName = formName + '[';
            compiledEventTree.documentrules = GetDocumentRules();
            compiledEventTree.sourcedocuments = GetSourceDocumentRules();
            return SerializedRuleTree(compiledEventTree);
        }
        public string SerializedRuleTree(RuleEventCompiler compiledEventTree)
        {
            string serializedJSON = "";
            serializedJSON = JsonConvert.SerializeObject(compiledEventTree);
            return serializedJSON;
        }

        public DocumentRules GetDocumentRules()
        {
            DocumentRules item = new DocumentRules();
            item.sourcesectionrules = GetSourceSection();
            item.targetsectionrules = GetTargetSection();

            return item;
        }

        public List<SourceSectionRules> GetSourceSection()
        {
            List<SourceSectionRules> items = new List<SourceSectionRules>();
            var startLen = formName.Length;
            var extractSource = (from m in _compiledDocuments
                                 where m.FormDesignVersionId == formDesignVersionId
                                 select new
                                 {
                                     ID = m.DocumentId,
                                     formdesignversionId = m.FormDesignVersionId,
                                     Rtype = m.DocumentType,
                                     Event = m.EventType,
                                     TargetSectionName = m.CompileRule.Target.TargetPath.Split('[')[1].Split('.')[0],
                                     SectionName = m.CompileRule.SourceContainer.RuleSources.Where(i => i.SourcePath.StartsWith(formName)).Select(sel => sel.SourcePath.Split('[')[1].Split('.')[0]).ToList()
                                 }).Where(i => i.SectionName != null && i.SectionName.Count > 0)
                                 .ToList();

            var sectionName = extractSource.SelectMany(m => m.SectionName).Distinct();

            foreach (var s in sectionName)
            {
                SourceSectionRules st = new SourceSectionRules();
                var sectionData = (from m in extractSource
                                   where m.SectionName.Contains(s) && m.TargetSectionName != s
                                   select new Rules
                                   {
                                       id = m.ID.ToString(),
                                       type = m.Rtype,
                                       events = m.Event.Split(','),
                                   })
                                   .ToList();
                st.section = s.ToString();
                st.rules = GetSectionWiseRules(sectionData);
                items.Add(st);
            }           
   
            return items;
        }

        public List<TargetSectionRules> GetTargetSection()
        {
            List<TargetSectionRules> items = new List<TargetSectionRules>();
            var startLen = formName.Length;
            var extractSource = (from m in _compiledDocuments
                                 where m.FormDesignVersionId == formDesignVersionId
                                 select new
                                 {
                                     ID = m.DocumentId,
                                     Rtype = m.DocumentType,
                                     Event = m.EventType,
                                     SectionName = m.CompileRule.Target.TargetPath.Split('[')[1].Split('.')[0],
                                     SectionFullName = m.CompileRule.Target.TargetPath
                                 }).Where(i => i.SectionFullName.StartsWith(formName))
                                 .ToList();

            var sectionName = extractSource.Select(m => m.SectionName).Distinct();

            foreach (var s in sectionName)
            {
                TargetSectionRules st = new TargetSectionRules();
                var sectionData = (from m in extractSource
                                   where m.SectionName == s
                                   select new Rules
                                   {
                                       id = m.ID.ToString(),
                                       type = m.Rtype,
                                       events = m.Event.Split(','),
                                   })
                                   .ToList();
                st.section = s.ToString();
                st.rules = GetSectionWiseRules(sectionData);
                items.Add(st);
            }

            return items;
        }

        public List<Rules> GetSectionWiseRules(List<Rules> data)
        {
            List<Rules> items = new List<Rules>();
            foreach (var d in data)
            {
                Rules r = new Rules();
                r.id = d.id;
                r.type = d.type;
                r.events = d.events;
                items.Add(r);
            }
            return items;
        }

        public List<SourceDocuments> GetSourceDocumentRules()
        {
            List<SourceDocuments> items = new List<SourceDocuments>();
            var startLen = formName.Length;
            var extractDesign = (from m in _compiledDocuments
                                 where m.FormDesignVersionId == formDesignVersionId
                                 select new
                                 {
                                     ID = m.DocumentId,
                                     DesignName = m.CompileRule.SourceContainer.RuleSources.Where(i => !i.SourcePath.StartsWith(formName)).Select(sel => sel.SourcePath.Split('[')[0]).ToList(),
                                     SectionName = m.CompileRule.SourceContainer.RuleSources.Where(i => !i.SourcePath.StartsWith(formName)).Select(sel => sel.SourcePath.Split('[')[1].Split('.')[0]).ToList()
                                 }).Where(i => i.SectionName != null && i.SectionName.Count > 0)
                                 .ToList();

            var listofDesign = extractDesign.SelectMany(s => s.DesignName).Distinct();
            foreach (var l in listofDesign)
            {
                SourceDocuments item = new SourceDocuments();
                item.document = l;
                item.sourcesectionrules = GetOtherSourceSection(l);
                items.Add(item);
            }
            return items;
        }

        public List<SourceSectionRules> GetOtherSourceSection(string designName)
        {
            List<SourceSectionRules> items = new List<SourceSectionRules>();
            var designappend = designName + "[";
            var extractSource = (from m in _compiledDocuments
                                 where m.FormDesignVersionId == formDesignVersionId
                                 select new
                                 {
                                     ID = m.DocumentId,
                                     formdesignversionId = m.FormDesignVersionId,
                                     Rtype = m.DocumentType,
                                     Event = m.EventType,
                                     SectionName = m.CompileRule.SourceContainer.RuleSources.Where(i => i.SourcePath.StartsWith(designappend)).Select(sel => sel.SourcePath.Split('[')[1].Split('.')[0]).ToList()
                                 }).Where(i => i.SectionName != null && i.SectionName.Count > 0)
                                 .ToList();

            var sectionName = extractSource.SelectMany(m => m.SectionName).Distinct();
            foreach (var s in sectionName)
            {
                SourceSectionRules item = new SourceSectionRules();
                var sectionData = (from m in extractSource
                                   where m.SectionName.Contains(s)
                                   select new Rules
                                   {
                                       id = m.ID.ToString(),
                                       type = m.Rtype,
                                       events = m.Event.Split(','),
                                   })
                                   .ToList();
                item.section = s.ToString();
                item.rules = GetSectionWiseRules(sectionData);
                items.Add(item);
            }
            return items;
        }

    }
}