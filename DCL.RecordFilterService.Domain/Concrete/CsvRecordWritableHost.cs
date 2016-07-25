using DCL.RecordFilterService.Domain.Abstract;
using System;

namespace DCL.RecordFilterService.Domain.Concrete
{
    public class CsvRecordWritableHost : IRecordWritableHost
    {
        /// <summary>
        /// Create a new CsvRecordWritableRepository.
        /// You will need to call CvsRecordWritableRepository.ChangeOutputDestination(string inputFileName, string outputFolder, string groupName)
        /// to set the output file location before calling CvsRecordWritableRepository.AddRecordToOutput().
        /// </summary>
        public IRecordWritableRepository CreateRecordWritableRepository()
        {
            return new CsvRecordWritableRepository();
        }

        /// <summary>
        /// Create a new CsvRecordWritableRepository
        /// </summary>
        public IRecordWritableRepository CreateRecordWritableRepository(string inputName, string outputContainerLocation, string groupName)
        {
            return new CsvRecordWritableRepository(inputName, outputContainerLocation, groupName);
        }
    }
}
