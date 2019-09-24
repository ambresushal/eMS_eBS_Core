using System;
namespace tmg.equinox.expressionbuilder
{
    interface ISectionSaveRuleOptimizer
    {
        bool hasPartChanged(string jsonPath);
        bool hasSectionChanged();
    }
}
