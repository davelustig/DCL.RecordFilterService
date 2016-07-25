using DCL.RecordFilterService.Domain.Abstract;
using System;
using System.Text;
using DCL.RecordFilterService.Domain.Entities;
using System.Collections.Concurrent;
using System.Timers;
using System.IO;

namespace DCL.RecordFilterService.Domain.Concrete
{
    public class CsvRecordWritableRepository : IRecordWritableRepository
    {
        #region Properties ----------------------------------------------------             
        /// <summary>
        /// The full path to the output file
        /// </summary>
        public string OutputLocation { get; set; }

        private bool _bufferOutput = true;
        /// <summary>
        /// Flag indicating whether output should be buffered before being written to file
        /// </summary>
        public bool BufferOutput
        {
            get { return _bufferOutput; }
            set { _bufferOutput = value; }
        }
        #endregion Properties -------------------------------------------------

        #region Private Variables and Members ---------------------------------
        private string _inputFileName;

        private string groupName;
        private string outputFolderPath;
        private string inputFileName
        {
            get { return _inputFileName; }
            set
            {
                _inputFileName = value;
                inputHasChanged = true;     // Flag that the input file has changed, and thus the output file should change to reflect this
            }
        }

        /// <summary>A queue of Records to write to the output file</summary>
        private ConcurrentQueue<Record> records = new ConcurrentQueue<Record>();

        /// <summary>Timer used to trigger writing buffered output to the output file</summary>
        private static Timer writeTimer = new Timer(50.0);

        /// <summary>
        /// Internal flag used to notify the code responsible for creating/updating the output file when it 
        /// should begin a new file/overwrite an existing one, vs continue appending to an existing one
        /// </summary>
        private bool inputHasChanged = true;

        /// <summary>The object used along with the lock() keyword to lock access by multiple threads to particular sections of code</summary>
        private Object myLock = new Object();
        #endregion Private Variables and Members ------------------------------

        #region Constructor ---------------------------------------------------
        /// <summary>
        /// A high-effiency manager for writing Records objects to a CSV file.  
        /// You will need to call ChangeOutputDestination(string inputFileName, string outputFolder, string groupName)
        /// to set the output file location before calling AddRecordToOutput().
        /// </summary>
        public CsvRecordWritableRepository()
        {
            writeTimer.AutoReset = true;
            writeTimer.Enabled = false;
            writeTimer.Elapsed += WriteTimer_Elapsed;
        }

        /// <summary>
        /// A high-effiency manager for writing Records objects to a CSV file
        /// </summary>
        /// <param name="inputFileName">The name of the input file, without its path</param>
        /// <param name="outputFolder">The path to the output folder</param>
        /// <param name="groupName">The name of the output group</param>
        public CsvRecordWritableRepository(String inputFileName, String outputFolderPath, String groupName)
        {
            ChangeOutputDestination(inputFileName, outputFolderPath, groupName);

            writeTimer.AutoReset = true;
            writeTimer.Enabled = false;
            writeTimer.Elapsed += WriteTimer_Elapsed;
        }
        #endregion Constructor ------------------------------------------------

        #region Interface Implementation --------------------------------------
        /// <summary>
        /// Update the output file path to reflect the new input file
        /// </summary>
        /// <param name="inputFileName">The name of the input file, without its path</param>
        public void ChangeOutputDestination(string inputFileName)
        {
            if (String.IsNullOrEmpty(inputFileName))
                throw new ArgumentNullException("inputFileName");
            if (inputFileName.Contains(@"\"))
                inputFileName = inputFileName.Substring(inputFileName.LastIndexOf(@"\") + 1);

            // If there is anything in the buffer, write it to the existing output file before switching to a new output file
            if(String.IsNullOrEmpty(OutputLocation) == false && records.IsEmpty == false)
                WriteRecordsToFile();

            this.inputFileName = inputFileName;

            OutputLocation = outputFolderPath + inputFileName.Substring(0, inputFileName.IndexOf(".csv")) + "_group" + groupName + ".csv";

            // Start the write timer so that WriteRecordsToFile() is executed at least once for this output file 
            //  (ie this is to create the file even when there are no records to write to it)
            writeTimer.Enabled = true;
        }

        /// <summary>
        /// Update the output file path to reflect the new input file, output folder, and/or group name
        /// </summary>
        /// <param name="inputFileName">The name of the input file, without its path</param>
        /// <param name="outputFolderPath">The path to the output folder</param>
        /// <param name="groupName">The name of the output group</param>
        public void ChangeOutputDestination(string inputFileName, string outputFolderPath, string groupName)
        {
            if (String.IsNullOrEmpty(outputFolderPath))
                throw new ArgumentNullException("outputFolder");
            if (String.IsNullOrEmpty(groupName))
                throw new ArgumentNullException("groupName");

            // Create the output folder if it does not exist
            this.outputFolderPath = outputFolderPath;
            if (this.outputFolderPath.EndsWith(@"\") == false)
                this.outputFolderPath += @"\";
            if (Directory.Exists(this.outputFolderPath) == false)
                Directory.CreateDirectory(this.outputFolderPath);

            this.groupName = groupName;

            // Only fully update the output file location if the input file name has been specified.
            //  Allow for the caller to not specify it at this point incase they don't yet know it.
            if(String.IsNullOrEmpty(inputFileName) == false)
                ChangeOutputDestination(inputFileName);
        }

        /// <summary>
        /// Add a Record to the output buffer, which will get written to the output file
        /// </summary>
        /// <param name="record">The Record to add</param>
        public void AddRecordToOutput(Record record)
        {
            if (BufferOutput)
            {
                records.Enqueue(record);

                // Start up the write timer (so this record will get written to file)
                writeTimer.Enabled = true;
            }
            else
                WriteRecordToFile(record);
        }

        /// <summary>
        /// Clean up this class when it is no longer needed
        /// </summary>
        public void Dispose()
        {
            writeTimer.Enabled = false;
            writeTimer.Dispose();

            // Try to ensure no buffered Records are not written to file
            if(records.IsEmpty == false)
                WriteRecordsToFile();
        }
        #endregion Interface Implementation -----------------------------------

        #region Helper Methods ------------------------------------------------
        /// <summary>
        /// Write the buffered Records to the output file
        /// </summary>
        private void WriteRecordsToFile()
        {
            // If the output file location hasn't been set yet, return and wait for the next call to write
            if (String.IsNullOrEmpty(OutputLocation))
                return;

            // This flag is used to indicate when we should append to an existing output file or create/overwrite a (new) file
            bool appendNotStartNewFile = true;

            lock (myLock)
            {
                // Create the output file if it doesn't already exist
                if (File.Exists(OutputLocation) == false || inputHasChanged == true)
                {
                    appendNotStartNewFile = false;
                    inputHasChanged = false;
                }

                // Create the text that should be written to the output file.
                // This is so we can write it all at once, instead of having one write per record.
                StringBuilder textToWrite = new StringBuilder();
                // If creating a new file, start the file with the field names
                if (appendNotStartNewFile == false)
                {   
                    Record record;
                    if(records.TryPeek(out record))
                        textToWrite.AppendLine(record.CsvFieldNamesString());
                }
                while (records.IsEmpty == false)
                {
                    Record record;
                    if (records.TryDequeue(out record))
                        textToWrite.AppendLine(record.ToCsvString());
                }

                // Open up the output file and write the buffered Records to it
                using (StreamWriter sw = new StreamWriter(OutputLocation, appendNotStartNewFile))
                {
                    sw.Write(textToWrite.ToString());
                }
            }
        }

        /// <summary>
        /// Write the passed record to the output file
        /// </summary>
        /// <remarks>This is not the best performing method, so its primary purpose is for testing</remarks>
        private void WriteRecordToFile(Record record)
        {
            // This flag is used to indicate when we should append to an existing output file or create/overwrite a (new) file
            bool appendNotStartNewFile = true;

            lock (myLock)
            {
                // Create the output file if it doesn't already exist
                if (File.Exists(OutputLocation) == false || inputHasChanged == true)
                {
                    appendNotStartNewFile = false;
                    inputHasChanged = false;
                }

                // Open up the output file and write the record to it
                using (StreamWriter sw = new StreamWriter(OutputLocation, appendNotStartNewFile))
                {
                    if (appendNotStartNewFile == false)
                        sw.WriteLine(record.CsvFieldNamesString());
                    sw.WriteLine(record.ToCsvString());
                }
            }
        }
        #endregion Helper Methods ---------------------------------------------

        #region Event Handlers ------------------------------------------------
        /// <summary>
        /// Write the buffered Records to the output file on schedule
        /// </summary>
        private void WriteTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            writeTimer.Enabled = false;

            WriteRecordsToFile();

            if(records.IsEmpty == false)
                writeTimer.Enabled = true;
        }
        #endregion Event Handlers ---------------------------------------------
    }
}
