using System;
namespace tmg.equinox.web.FormInstanceProcessor.ExpressionBuilder
{
    interface ISectionSaveRuleOptimizer
    {
        bool hasPartChanged(string jsonPath);
        bool hasSectionChanged();
    }
}
