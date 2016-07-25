using DCL.RecordFilterService.Configuration.Elements;
using System;

namespace DCL.CustomFilterService.Logic.Conditions
{
    public static class ConditionFactory
    {
        /// <summary>
        /// Get the correct implementation of the ICondition interface based on the conditionConfiguration's Type property
        /// </summary>
        /// <param name="conditionConfig">The configuration information used to specify what condition is needed</param>
        public static ICondition GetCondition(ConditionElement conditionConfig)
        {
            switch (conditionConfig.Type)
            {
                case ConditionType.isAllowed:
                    return new AllowedCondition(conditionConfig.Field, conditionConfig.Value);
                case ConditionType.isDuplicate:
                    return new DuplicateCondition(conditionConfig.Field);
                case ConditionType.isInRange:
                    return new RangedCondition(conditionConfig.Field, conditionConfig.RangeStart, conditionConfig.RangeEnd);
                case ConditionType.allInclusive:
                    return new AllInclusiveCondition();
                default:
                    throw new NotImplementedException("A condition of type '" + conditionConfig.Type + "' was specified, but no class has been implemented for it.  Supported conditions are isAllowed, isDuplicate, isInRange, and allInclusive.");
            }
        }
    }
}
