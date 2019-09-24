using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.web.Framework.Caching;
using tmg.equinox.ruleinterpreter.model;
using tmg.equinox.web.FormInstanceProcessor;

namespace tmg.equinox.web.FormDesignManager
{
    public class CompiledDocumentRuleReadOnlyObjectCache
    {
        private static Dictionary<int, CompiledDocumentRule> DocumentRuleList = new Dictionary<int, CompiledDocumentRule>();

        private static object lockObject = new object();
        private CompiledDocumentRuleReadOnlyObjectCache()
        {

        }

        public static CompiledDocumentRule GetCompiledDocumentRule(int documentRuleId, int tenantId, int folderVersionId, IUIElementService uiElementService, IFormDesignService formDesignService)
        {
            CompiledDocumentRule rule = null;
            lock (lockObject)
            {
                if (DocumentRuleList.ContainsKey(documentRuleId) == true)
                {
                    rule = DocumentRuleList[documentRuleId];
                }
                else
                {
                    ExpressionBuilderEventMapReader expressionEventMapper = new ExpressionBuilderEventMapReader(tenantId, folderVersionId, uiElementService, formDesignService);
                    rule = expressionEventMapper.GetCompiledRule(documentRuleId);
                    DocumentRuleList.Add(documentRuleId, rule);
                }
            }
            return rule;
        }

        public static void RemoveDoucmentRuleFromCache(int documentRuleId)
        {
            DocumentRuleList.Remove(documentRuleId);
        }

        public static void CleanDocumentRule()
        {
            DocumentRuleList.Clear();
        }
    }
}
