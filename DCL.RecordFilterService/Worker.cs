using DCL.RecordFilterService.Configuration;
using DCL.RecordFilterService.Domain.Abstract;
using DCL.CustomFilterService.Logic;
using System;
using System.Diagnostics;

namespace DCL.RecordFilterService
{
    public class Worker: IDisposable
    {
        private IRecordReadableRepository inputRepo;
        private ActionProcessor actionProcessor;

        public Worker(IRecordReadableRepository inputRepo, IRecordWritableHost outputRepoHost, CustomFilterServiceSection config)
        {
            this.inputRepo = inputRepo;

            this.actionProcessor = new ActionProcessor(config, outputRepoHost);

            inputRepo.UnprocessedFileAvailable += Repo_UnprocessedFileAvailable;

            inputRepo.Start();
        }

        private void Repo_UnprocessedFileAvailable(object sender, Domain.Concrete.UnprocessedFileEventArgs e)
        {
            while (inputRepo.Next())
            {
                Stopwatch sw = new Stopwatch();
                Console.WriteLine("Starting processing of file " + inputRepo.CurrentFilePath);
                sw.Start();

                actionProcessor.ChangeOutput(inputRepo.CurrentFileName, inputRepo.CurrentCustomer, inputRepo.CurrentFileType);
                actionProcessor.ProcessActions(inputRepo.Records, inputRepo.CurrentCustomer, inputRepo.CurrentFileType);

                sw.Stop();
                Console.WriteLine(sw.Elapsed);
            }
        }

        /// <summary>
        /// Cleanup the resources used by this object when it is no longer needed
        /// </summary>
        public void Dispose()
        {
            inputRepo.Dispose();
            actionProcessor.Dispose();
        }
    }
}
