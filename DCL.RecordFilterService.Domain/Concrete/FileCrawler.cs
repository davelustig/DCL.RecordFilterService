using System;
using System.IO;
using System.Linq;
using System.Collections.Concurrent;
using System.Timers;

namespace DCL.RecordFilterService.Domain.Concrete
{
    public class FileCrawler: IDisposable
    {
        #region Properties ----------------------------------------------------
        /// <summary>The path of the folder hosting all the files to be processed</summary>
        private string InputFolder { get; set; }

        /// <summary>Thread-safe queue for holding the file paths of all files that need to be processed</summary>
        private ConcurrentQueue<string> unprocessedFilePaths = new ConcurrentQueue<string>();

        /// <summary>The object used along with the lock() keyword to lock access by multiple threads to particular sections of code</summary>
        private Object myLock = new Object();

        private static Timer eventTimer = new Timer(50.0);
        #endregion Properties -------------------------------------------------

        #region Members -------------------------------------------------------
        private FileSystemWatcher FileWatcher = new FileSystemWatcher();
        #endregion Members ----------------------------------------------------

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
        /// The File Crawler notifies listeners of all files available for processing within the configured directory
        /// </summary>
        public FileCrawler(): this(Configuration.Current.CustomFilterServiceConfig.InputFolder)
        {
        }
        
        /// <summary>
        /// The File Crawler notifies listeners of all files available for processing within the specified directory
        /// </summary>
        /// <param name="inputFolder">The directory to monitor</param>
        public FileCrawler(string inputFolder)
        {
            InputFolder = inputFolder;

            if (Directory.Exists(InputFolder) == false)
                throw new DirectoryNotFoundException("The input directory '" + InputFolder + "' could not be located.  Please verify it exists.");

            eventTimer.AutoReset = false;
            eventTimer.Enabled = false;
            eventTimer.Elapsed += EventTimer_Elapsed;
        }
        #endregion Constructors -----------------------------------------------

        #region Public Methods ------------------------------------------------
        /// <summary>
        /// Start up the File Crawler.  This loads up the Unprocessed Files queue, hooks up the File Watcher events,
        /// and fires an UnprocessedFilesAvailable event if appropriate.  This is separate from the constructor to 
        /// allow the caller to fully setup all required classes before it starts receiveing events.
        /// </summary>
        public void Start()
        {
            if (Directory.Exists(InputFolder) == false)
                return;

            // Empty the queue
            while (unprocessedFilePaths.IsEmpty == false)
            {
                string tmp;
                unprocessedFilePaths.TryDequeue(out tmp);
            }

            // Add the file paths of all CSV files in the input folder to the UnprocessedFilePaths queue
            string[] filePaths = Directory.GetFiles(InputFolder, "*.csv", SearchOption.TopDirectoryOnly);
            foreach (string filePath in filePaths)
            {
                if(IsInterestingFile(filePath.Substring(filePath.LastIndexOf(@"\"))))
                    unprocessedFilePaths.Enqueue(filePath);
            }

            // Configure the file watcher to fire events when the contents of the input folder change
            FileWatcher.Path = InputFolder;
            FileWatcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.Size;
            FileWatcher.Filter = "*.csv";
            FileWatcher.Changed += FileWatcher_Changed;
            FileWatcher.Created += FileWatcher_Created;
            FileWatcher.Deleted += FileWatcher_Deleted;
            FileWatcher.Renamed += FileWatcher_Renamed;

            FileWatcher.EnableRaisingEvents = true;

            // If there are files to process, fire the UnprocessedFilesAvailable event
            if (unprocessedFilePaths.Count > 0)
                OnUnprocessedFileAvailable(new UnprocessedFileEventArgs());
        }

        /// <summary>
        /// Pause the File Crawler.  This turns the File Watcher off, such that new events will not be fired.  
        /// The Unprocessed Files queue is not affected.
        /// </summary>
        public void Pause()
        {
            FileWatcher.EnableRaisingEvents = false;
        }

        /// <summary>
        /// Unpause the File Crawler.  This turns the File Watcher on, such that events will be fired when the contents of the input directory change.  
        /// The Unprocessed Files queue is not affected.
        /// </summary>
        public void UnPause()
        {
            FileWatcher.EnableRaisingEvents = false;
        }

        /// <summary>
        /// Stop the File Crawler.  This empties the Unprocessed Files queue, unhooks the File Watcher events, 
        /// and turns the File Watcher off.  Use this if you want to change the input folder restart the crawler.
        /// </summary>
        public void Stop()
        {
            // Empty the queue
            while (unprocessedFilePaths.IsEmpty == false)
            {
                string tmp;
                unprocessedFilePaths.TryDequeue(out tmp);
            }

            FileWatcher.EnableRaisingEvents = false;

            FileWatcher.Changed -= FileWatcher_Changed;
            FileWatcher.Created -= FileWatcher_Created;
            FileWatcher.Deleted -= FileWatcher_Deleted;
            FileWatcher.Renamed -= FileWatcher_Renamed;
        }

        /// <summary>
        /// Determine whether there are files that still need to be processed
        /// </summary>
        public bool AreThereUnprocessedFiles()
        {
            return (unprocessedFilePaths.IsEmpty == false);
        }

        /// <summary>
        /// Get the file path of the next unprocessed file, or null if there are none left
        /// </summary>
        public string NextUnprocessedFile()
        {
            string filePath = null;

            if (unprocessedFilePaths.IsEmpty == false)
                unprocessedFilePaths.TryDequeue(out filePath);
            
            return filePath;
        }

        /// <summary>
        /// Cleanup the resources used by this object when it is no longer needed
        /// </summary>
        public void Dispose()
        {
            Stop();

            FileWatcher.Dispose();

            eventTimer.Dispose();

            unprocessedFilePaths = null;
        }
        #endregion Public Methods ---------------------------------------------

        #region Helper Methods ------------------------------------------------
        /// <summary>
        /// Remove a specific string from the queue.
        /// Note: This may affect the ordering of the queue.
        /// </summary>
        /// <param name="filePathToRemove">The string to remove</param>
        private void RemoveEnqueuedString(string filePathToRemove)
        {
            if (unprocessedFilePaths.Contains(filePathToRemove))
            {
                // The max count and i variables are used to prevent the while loop from getting into an infinite 
                // loop if another process removes the file path before this one does
                int maxCount = 0;
                int i = 0;
                string filePath = string.Empty;

                lock (myLock)
                {
                    maxCount = unprocessedFilePaths.Count;
                    unprocessedFilePaths.TryDequeue(out filePath);

                    while (filePath.Equals(filePathToRemove, StringComparison.OrdinalIgnoreCase) == false && i < maxCount)
                    {
                        unprocessedFilePaths.Enqueue(filePath);
                        unprocessedFilePaths.TryDequeue(out filePath);

                        i++;
                    }
                }
            }
        }

        /// <summary>
        /// Check if the file name is worth monitoring
        /// </summary>
        /// <param name="fileName">The name of the file</param>
        private bool IsInterestingFile(string fileName)
        {
            // If the file name follows the convention [customer]_[filetype]_[date].csv
            if (fileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase) && fileName.Contains("_"))
            {
                String[] fileNameParts = fileName.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                if(fileNameParts.Length == 3)
                    return true;
            }

            return false;
        }
        #endregion Helper Methods ---------------------------------------------

        #region Event Handlers ------------------------------------------------
        /// <summary>
        /// When an input file is changed, add it back to the Unprocessed Files queue
        /// </summary>
        private void FileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (unprocessedFilePaths.Contains(e.FullPath) == false && IsInterestingFile(e.Name))
            {
                unprocessedFilePaths.Enqueue(e.FullPath);
                // Fire the event notifing listeners that another file is available for processing
                //OnUnprocessedFileAvailable(new UnprocessedFileEventArgs());
                eventTimer.Enabled = true;
            }
        }

        /// <summary>
        /// When a new file is created in the input directory, add it to the Unprocessed Files queue
        /// </summary>
        private void FileWatcher_Created(object sender, FileSystemEventArgs e)
        {
            if (unprocessedFilePaths.Contains(e.FullPath) == false && IsInterestingFile(e.Name))
            {
                unprocessedFilePaths.Enqueue(e.FullPath);
                // Fire the event notifing listeners that another file is available for processing
                //OnUnprocessedFileAvailable(new UnprocessedFileEventArgs());
                eventTimer.Enabled = true;
            }
        }

        /// <summary>
        /// When an input file is deleted, remove it from the Unprocessed Files queue
        /// </summary>
        private void FileWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            RemoveEnqueuedString(e.FullPath);
        }

        /// <summary>
        /// When an input file is renamed, update is path in the queue, if present
        /// </summary>
        private void FileWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            RemoveEnqueuedString(e.OldFullPath);

            if (IsInterestingFile(e.Name))
                unprocessedFilePaths.Enqueue(e.FullPath);
        }

        /// <summary>
        /// Fire the UnprocessedFileAvailable event after a delay.  This is to account for the fact
        /// that one change to a file can result in multiple Changed events firing, and we don't want 
        /// to reprocess the file for each one.
        /// </summary>
        private void EventTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            eventTimer.Enabled = false;

            if (unprocessedFilePaths.IsEmpty == false)
                OnUnprocessedFileAvailable(new UnprocessedFileEventArgs());
        }
        #endregion Event Handlers ---------------------------------------------

    }
}
