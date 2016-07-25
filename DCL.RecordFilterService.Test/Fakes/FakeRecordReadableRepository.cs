using DCL.RecordFilterService.Domain.Abstract;
using System;
using System.Collections.Generic;
using DCL.RecordFilterService.Domain.Concrete;
using DCL.RecordFilterService.Domain.Entities;

namespace DCL.RecordFilterService.Test.Fakes
{
    public class FakeRecordReadableRepository : IRecordReadableRepository
    {
        #region Properties and Members ----------------------------------------
        public string CurrentFileName
        {
            get
            {
                return "Fake.csv";
            }
        }

        public string CurrentFilePath { get; set; }

        private static int fileCounter = 0;
        private List<Record> _records = null;

        /// <summary>
        /// Iterate through a random list of Records
        /// </summary>
        public IEnumerable<Record> Records
        {
            get
            {
                if(_records == null)
                {
                    _records = new List<Record>(434520);
                    GenerateRandomRecords();
                }
                fileCounter++;
                return _records;
            }
        }

        public string CurrentCustomer
        {
            get { return "Fake"; }
        }

        public string CurrentFileType
        {
            get { return "PeopleRecords"; }
        }

        public string CurrentFileDate
        {
            get { return "20160724"; }
        }
        #endregion Properties and Members -------------------------------------

        #region Custom Events -------------------------------------------------
        public event UnprocessedFileAvailableEventHandler UnprocessedFileAvailable;

        /// <summary>
        /// Method used to raise the UnprocessedFileAvailable event, if any listeners are attached to it
        /// </summary>
        /// <param name="e">The event arguments to pass to any event handlers hooked up to the event</param>
        protected virtual void OnUnprocessedFileAvailable(UnprocessedFileEventArgs e)
        {
            UnprocessedFileAvailable?.Invoke(this, e);
        }
        #endregion Custom Events ----------------------------------------------

        #region Public Methods ------------------------------------------------
        /// <summary>
        /// Return true only twice to simulate two unprocessed files available
        /// </summary>
        public bool AreThereUnprocessedFiles()
        {
            return (fileCounter < 2);
        }

        /// <summary>
        /// Move on to the next file to process.  After calling this method, CurrentFilePath 
        /// will be updated and Records should start to return data from the file it references.
        /// </summary>
        /// <returns>True=There is a next file to process; False=No next file</returns>
        public bool Next()
        {
            return AreThereUnprocessedFiles();
        }

        /// <summary>
        /// Start firing events on files that need to be processed
        /// </summary>
        public void Start()
        {
            OnUnprocessedFileAvailable(new UnprocessedFileEventArgs());
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }

        public void UnPause()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Cleanup the resources used by this object when it is no longer needed
        /// </summary>
        public void Dispose()
        {
            _records.Clear();
            _records = null;
        }
        #endregion Public Methods ---------------------------------------------

        #region Helper Methods ------------------------------------------------
        /// <summary>
        /// Populate the Records list with psuedorandom entries
        /// </summary>
        private void GenerateRandomRecords()
        {
            string[] firstNames = new string[] { "David", "Sam", "Bob", "Wendy", "Ralph", "George", "Matt", "John", "Jacky", "Eddy" };
            string[] lastNames = new string[] { "Davis", "Rogers", "Hemmingway", "Thomson", "Lee", "Cameron", "Patak", "Hannaford", "Xu" };
            string[] genders = new string[] { "M", "F" };
            Random rand = new Random();

            for(int i = 0; i < _records.Capacity; i++)
            {
                Record record = new Record();
                record["FirstName"] = firstNames[i % 10];
                record["LastName"] = lastNames[i % 9];
                record["Age"] = (i % 100).ToString();
                record["PhoneNumber"] = rand.Next(10000, 99999).ToString() + rand.Next(10000, 99999).ToString();  // Generate a random 10-digit number
                record["Gender"] = genders[i % 2];

                _records.Add(record);
            }
        }
        #endregion Helper Methods ---------------------------------------------
    }
}
