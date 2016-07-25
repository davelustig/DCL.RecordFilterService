using DCL.RecordFilterService.Domain.Entities;
using System;

namespace DCL.CustomFilterService.Logic.Conditions
{
    public interface ICondition
    {
        bool IsMet(Record record);
        void Reset();
    }
}
