using DCL.RecordFilterService.Domain.Concrete;
using DCL.RecordFilterService.Domain.Entities;
using System;
using System.Collections.Generic;

namespace DCL.RecordFilterService.Domain.Abstract
{
    public interface IRecordReadableRepository: IDisposable
    {
        string CurrentFilePath { get; set; }
        string CurrentFileName { get; }
        string CurrentCustomer { get; }
        string CurrentFileType { get; }
        string CurrentFileDate { get; }

        IEnumerable<Record> Records { get; }

        bool AreThereUnprocessedFiles();
        bool Next();

        void Start();
        void Stop();
        void Pause();
        void UnPause();

        event UnprocessedFileAvailableEventHandler UnprocessedFileAvailable;
    }
}
