using System;

namespace DCL.RecordFilterService.Domain.Concrete
{
    /// <summary>
    /// The Event arguments passed to all handlers of the UnprocessedFileAvailable event
    /// </summary>
    public class UnprocessedFileEventArgs: EventArgs
    {
        public string FilePath { get; set; }
    }

    /// <summary>
    /// The method structure that any UnprocessedFileAvailable event handler must conform to
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void UnprocessedFileAvailableEventHandler(object sender, UnprocessedFileEventArgs e);
}
