using System;
namespace tmg.equinox.infrastructure.util
{
    interface ISectionSaveRuleOptimizer
    {
        bool hasPartChanged(string jsonPath);
        bool hasSectionChanged();
    }
}
