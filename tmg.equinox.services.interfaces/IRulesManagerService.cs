using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.RulesManager;
using tmg.equinox.applicationservices.viewmodels.UIElement;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IRulesManagerService
    {
        List<FormDesignViewModel> GetFormDesigns(int tenandId);
        List<FormDesignViewModel> GetFormDesignVersions(int tenantId, int formDesignId);
        IEnumerable<ElementRowModel> GetElementList(int tenantId, int formDesignVersionId);
        List<RuleRowViewModel> GetRulesByFormDesignVersion(int tenantId, int formDesignVersionId);
        GridPagingResponse<RuleRowViewModel> GetRulesByFormDesignVersion(int tenantId, int formDesignVersionId, GridPagingRequest gridPagingRequest);
        List<RuleRowViewModel> GetRulesByFormDesignVersionByTarget(int tenantId, int formDesignVersionId);
        GridPagingResponse<RuleRowViewModel> GetRulesByFormDesignVersionByTarget(int tenantId, int formDesignVersionId, GridPagingRequest gridPagingRequest);
        List<RuleRowViewModel> GetRulesByFormDesignVersionBySource(int tenantId, int formDesignVersionId);
        GridPagingResponse<RuleRowViewModel> GetRulesByFormDesignVersionBySource(int tenantId, int formDesignVersionId, GridPagingRequest gridPagingRequest);
        RuleRowModel GetRuleByID(int tenantId, int ruleId, int formDesignVersionId);
        ServiceResult AddRule(int tenantId, string userName, RuleRowModel model);
        ServiceResult UpdateRule(int tenantId, string userName, RuleRowModel rule);
        ServiceResult DeleteRule(int tenantId, int[] rules, int formDesignVersionId);
        List<RuleRowViewModel> GetSourceByRuleID(int tenantId, int formDesignVersionId, int ruleId);
        List<RuleRowViewModel> GetTargetByRuleID(int tenantId, int formDesignVersionId, int ruleId);
        ServiceResult AssignTargets(int tenantId, int uiElementId, List<ElementRuleMap> elementMap, string userName);
        List<ElementRuleMap> GetTargetMapByRuleID(int tenantId, int ruleId);
        List<RepeaterKeyFilterModel> GetKeyFilter(int ruleId, bool isRightOperand);
        List<ProductTestDataModel> GetOperandNames(int tenantId, int formDesignVersionId, List<ProductTestDataModel> operands);
        List<RuleHierarchyViewModel> GetRulesHierarchyByFormDesignVersion(int tenantId, int formDesignVersionId);

    }
}
