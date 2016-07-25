using System;

namespace DCL.RecordFilterService.Domain.Abstract
{
    public interface IRecordWritableHost
    {
        IRecordWritableRepository CreateRecordWritableRepository();
        IRecordWritableRepository CreateRecordWritableRepository(string inputName, string outputContainerLocation, string groupName);
    }
}
