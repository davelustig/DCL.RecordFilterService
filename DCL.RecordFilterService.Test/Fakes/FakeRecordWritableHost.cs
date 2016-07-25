using DCL.RecordFilterService.Domain.Abstract;
using System;

namespace DCL.RecordFilterService.Test.Fakes
{
    /// <summary>
    /// Implementation of the IRecordWritableHost interface to be used for testing
    /// </summary>
    public class FakeRecordWritableHost : IRecordWritableHost
    {
        /// <summary>
        /// Create a new FakeRecordWritableRepository
        /// </summary>
        public IRecordWritableRepository CreateRecordWritableRepository()
        {
            return new FakeRecordWritableRepository();
        }

        /// <summary>
        /// Create a new FakeRecordWritableRepository
        /// </summary>
        public IRecordWritableRepository CreateRecordWritableRepository(string inputName, string outputContainerLocation, string groupName)
        {
            return new FakeRecordWritableRepository();
        }
    }
}
