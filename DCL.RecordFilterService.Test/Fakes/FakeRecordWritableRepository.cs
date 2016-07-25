using DCL.RecordFilterService.Domain.Abstract;
using System;
using System.Collections.Generic;
using DCL.RecordFilterService.Domain.Entities;

namespace DCL.RecordFilterService.Test.Fakes
{
    /// <summary>
    /// Implementation of the IRecordWritableRepository interface to be used for testing
    /// </summary>
    public class FakeRecordWritableRepository : IRecordWritableRepository
    {
        #region Properties and Members ----------------------------------------
        public string OutputLocation { get; set; }
        
        /// <summary>List used to store output for testing, instead of writing to to a file</summary>
        public List<Record> Records = new List<Record>();
        #endregion Properties and Members -------------------------------------

        #region Public Methods ------------------------------------------------
        /// <summary>
        /// Add a Record to the output buffer
        /// </summary>
        /// <param name="record">The Record to add</param>
        public void AddRecordToOutput(Record record)
        {
            Records.Add(record);
        }

        /// <summary>
        /// Update the output location to reflect the new input name
        /// </summary>
        /// <param name="inputName">The name of the input file source, without its context/path</param>
        public void ChangeOutputDestination(string inputName)
        {
            OutputLocation = inputName;
        }

        /// <summary>
        /// Update the output location to reflect the new input name, output folder, and/or group name
        /// </summary>
        /// <param name="inputName">The name of the input source, without its context/path</param>
        /// <param name="outputContainerLocation">The location of the output folder, database, etc</param>
        /// <param name="groupName">The name of the output group</param>
        public void ChangeOutputDestination(string inputName, string outputContainerLocation, string groupName)
        {
            OutputLocation = outputContainerLocation + groupName + inputName;
        }

        /// <summary>
        /// Clean up this class when it is no longer needed
        /// </summary>
        public void Dispose()
        {
            Records.Clear();
            Records = null;
        }
        #endregion Public Methods ---------------------------------------------
    }
}
