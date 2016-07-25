using DCL.RecordFilterService.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using DCL.RecordFilterService.Domain.Entities;
using System.IO;
using System.Threading;

namespace DCL.RecordFilterService.Domain.Concrete
{
    public class CsvRecordReadableRepository : IRecordReadableRepository
    {
        #region Members and Properties ----------------------------------------
        /// <summary>The File Crawler is used to notify this class when new files are available for processing</summary>
        private FileCrawler fileCrawler;

        /// <summary>String array used to hold a portion of the CSV file for processing</summary>
        private string[] lineBuffer = new string[100];
        private bool endOfFileReached = true;

        /// <summary>An IEnumerable interface to the current file, used to read each line in one at a time</summary>
        private IEnumerable<string> fileLines = null;

        /// <summary>The CVS fields hosted in the current file</summary>
        private String fileFields = String.Empty;

        private string _currentFilePath;
        /// <summary>The path of the file currently being processed</summary>
        public string CurrentFilePath
        {
            get
            {
                return _currentFilePath;
            }
            set
            {
                _currentFilePath = value;

                CurrentFileName = (value != null) ? _currentFilePath.Substring(_currentFilePath.LastIndexOf(@"\") + 1) : String.Empty;
            }
        }

        private string _currentFileName;
        /// <summary>The name of the file currently being processed</summary>
        public string CurrentFileName
        {
            get { return _currentFileName; }
            private set
            {
                _currentFileName = value;

                if (_currentFileName != null && _currentFileName.Contains('.'))
                {
                    string[] fileParts = _currentFileName.Substring(0, _currentFileName.LastIndexOf('.')).Split('_');
                    CurrentCustomer = (fileParts.Length > 0) ? fileParts[0] : String.Empty;
                    CurrentFileType = (fileParts.Length > 1) ? fileParts[1] : String.Empty;
                    CurrentFileDate = (fileParts.Length > 2) ? fileParts[2] : String.Empty;
                }
                else
                    CurrentCustomer = CurrentFileType = CurrentFileDate = String.Empty;
            }
        }

        /// <summary>The customer associated with the file currently being processed</summary>
        public string CurrentCustomer { get; private set; }
        /// <summary>The file record classification of the file currently being processed</summary>
        public string CurrentFileType { get; private set; }
        /// <summary>The date of the file record being processed</summary>
        public string CurrentFileDate { get; private set; }

        /// <summary>
        /// Iterate through all Records stored in the file referenced by CurrentFilePath
        /// </summary>
        IEnumerable<Record> IRecordReadableRepository.Records
        {
            get
            {
                while(endOfFileReached == false)
                {
                    LoadBuffer();

                    for(int i = 0; i < lineBuffer.Length; i++)
                    {
                        if(string.IsNullOrEmpty(lineBuffer[i]) == false)
                            yield return new Record(fileFields, lineBuffer[i]);
                    }
                }

                // When we are done iterating through a file, reset the current file path
                CurrentFilePath = null;
            }
        }
        #endregion Members and Properties -------------------------------------

        #region Custom Events -------------------------------------------------
        /// <summary>
        /// Event raised when an unprocessed file is available for processing
        /// </summary>
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

        #region Constructors --------------------------------------------------
        /// <summary>
        /// Create a new CsvRecordReadableRepository, which is used to read and parse CSV files in an input directory,
        /// as well as notify any listeners of changes.
        /// </summary>
        /// <param name="inputFolder">The directory hosting all the input files containing the records to process</param>
        /// <param name="startFileProcessing">True=Start firing events when new files are available for processing; False=Wait</param>
        public CsvRecordReadableRepository(string inputFolder, Boolean startFileProcessing)
        {
            fileCrawler = new FileCrawler(inputFolder);

            fileCrawler.UnprocessedFileAvailable += FileCrawler_UnprocessedFileAvailable;

            if (startFileProcessing == true)
                fileCrawler.Start();
        }
        #endregion Constructors -----------------------------------------------

        #region Public Methods ------------------------------------------------
        /// <summary>
        /// Start firing events on files that need to be processed
        /// </summary>
        public void Start()
        {
            fileCrawler.Start();
        }

        /// <summary>
        /// Pause event firing temporarily
        /// </summary>
        public void Pause()
        {
            fileCrawler.Pause();
        }

        /// <summary>
        /// Unpause event firing
        /// </summary>
        public void UnPause()
        {
            fileCrawler.UnPause();
        }

        /// <summary>
        /// Stop firing events on files that need to be processed.  Only use this when changing input folders, 
        /// as it will cause all existing files to be reprocessed when Start is called.
        /// </summary>
        public void Stop()
        {
            fileCrawler.Stop();
        }

        /// <summary>
        /// Determine whether there are files that still need to be processed
        /// </summary>
        public bool AreThereUnprocessedFiles()
        {
            return fileCrawler.AreThereUnprocessedFiles();
        }

        /// <summary>
        /// Move on to the next file to process.  After calling this method, CurrentFilePath 
        /// will be updated and Records should start to return data from the file it references.
        /// </summary>
        /// <returns>True=There is a next file to process; False=No next file</returns>
        public bool Next()
        {
            if (AreThereUnprocessedFiles() == false)
                return false;

            CurrentFilePath = fileCrawler.NextUnprocessedFile();
            endOfFileReached = false;

            return true;
        }

        /// <summary>
        /// Cleanup the resources used by this object when it is no longer needed
        /// </summary>
        public void Dispose()
        {
            lineBuffer = null;
            fileCrawler.Dispose();
        }
        #endregion Public Methods ---------------------------------------------

        #region Helper Methods ------------------------------------------------
        /// <summary>
        /// Populate the lineBuffer from the current file.  This may need to be called multiple times to 
        /// read in the entire file (ie the line buffer does not host the entire file, just parts of it)
        /// </summary>
        /// <exception cref="Exception">This method can throw many exceptions.  Prepare for them...</exception>
        private void LoadBuffer()
        {
            if (fileLines == null)
            {
                int tryCount = 0;

                // Make multiple attempts to read from the file if the initial attempts fail
                while (tryCount < 5)
                {
                    try
                    {
                        // Prevent the OnUnprocessedFileAvailable event from being fired if processing a file
                        endOfFileReached = false;

                        fileLines = File.ReadLines(CurrentFilePath);

                        // Move the enumerator to the first line to retrieve the field names
                        var fileEnumerator = fileLines.GetEnumerator();
                        fileEnumerator.MoveNext();
                        fileFields = fileEnumerator.Current;

                        break;
                    }
                    catch (IOException)
                    {
                        if (tryCount != 4)
                        {
                            // Sleep for a millisecond, hoping that the file will be ready for reading afterwards
                            Thread.Sleep(1);
                            tryCount++;
                        }
                        else
                        {
                            // If unable to process a file, allow the OnUnprocessedFileAvailable event to be fired again
                            endOfFileReached = true;
                            throw;
                        }
                    }
                }
            }

            // Populate the line buffer with lines from the file
            var enumerator = fileLines.GetEnumerator();

            for (int i = 0; i < lineBuffer.Length; i++)
            {   // If there is another line in the file to read, read it into the buffer
                if (enumerator.MoveNext() == true)
                    lineBuffer[i] = enumerator.Current;
                // Else, clear the corresponding line in the buffer
                else
                    lineBuffer[i] = null;
            }

            // If there is nothing left in the file to read...
            if (lineBuffer[lineBuffer.Length -1] == null && enumerator.Current == null)
            {
                // Dispose of the enumerator, reset the file reader, and notify others that the end of the file has been reached
                enumerator.Dispose();
                fileLines = null;
                endOfFileReached = true;
            }            
        }
        #endregion Helper Methods ---------------------------------------------

        #region Event Handlers ------------------------------------------------
        /// <summary>
        /// When the FileCrawler notifies us that there are unprocessed files available, inform any subscribers to
        /// the CvsRecordReadableRepository's UnproccessedFileAvailable event
        /// </summary>
        private void FileCrawler_UnprocessedFileAvailable(object sender, UnprocessedFileEventArgs e)
        {
            // Only pass the event up to any subscribers is no files are currently being processed
            if(endOfFileReached == true && CurrentFilePath == null)
                OnUnprocessedFileAvailable(e);
        }
        #endregion Event Handlers ---------------------------------------------
    }
}
