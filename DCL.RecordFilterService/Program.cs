using DCL.RecordFilterService.Domain.Concrete;
using System;
using Current = DCL.RecordFilterService.Configuration.Current;

namespace DCL.RecordFilterService
{
    class Program
    {
        static Worker worker;

        static void Main(string[] args)
        {
            try
            {
                CsvRecordReadableRepository inputRepo = new CsvRecordReadableRepository(Current.CustomFilterServiceConfig.InputFolder, false);
                CsvRecordWritableHost outputRepoHost = new CsvRecordWritableHost();
                //Test.Fakes.FakeRecordReadableRepository inputRepo = new Test.Fakes.FakeRecordReadableRepository();
                //Test.Fakes.FakeRecordWritableHost outputRepoHost = new Test.Fakes.FakeRecordWritableHost();
                using (worker = new Worker(inputRepo, outputRepoHost, Current.CustomFilterServiceConfig))
                {
                    // Busy wait, allowing the Worker to process events as they come in
                    while (true) { }
                }
            }
            catch(Exception ex)
            {
                // Placeholder for error logging
                //ErrorLogger.WriteError(Current.CustomFilterServiceConfig.LogFolder, ex.ToString());

                // Until error logging has been implemented, throw the error so perhaps the OS will log it
                throw;
            }
        }
    }
}
