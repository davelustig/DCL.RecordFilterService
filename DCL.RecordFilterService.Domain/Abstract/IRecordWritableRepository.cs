using DCL.RecordFilterService.Domain.Entities;
using System;

namespace DCL.RecordFilterService.Domain.Abstract
{
    public interface IRecordWritableRepository: IDisposable
    {
        string OutputLocation { get; set; }

        void AddRecordToOutput(Record record);
        void ChangeOutputDestination(string inputName);
        void ChangeOutputDestination(string inputName, string outputContainerLocation, string groupName);
    }
}
