using DCL.RecordFilterService.Configuration;
using DCL.RecordFilterService.Configuration.Elements;
using DCL.RecordFilterService.Domain.Abstract;
using DCL.RecordFilterService.Domain.Entities;
using DCL.CustomFilterService.Logic.Actions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DCL.CustomFilterService.Logic
{
    public class ActionProcessor : IDisposable
    {
        private List<Actions.Action> actions = new List<Actions.Action>();

        #region Constructor ---------------------------------------------------
        /// <summary>
        /// Create a new Action Processor, which will compile the configured actions and conditions and process collections of Records against them
        /// </summary>
        /// <param name="config">The configuration containing which actions and conditions should be used when processing a Record</param>
        /// <remarks>ChangeOutput() must be called before ProcessActions()</remarks>
        public ActionProcessor(CustomFilterServiceSection config, IRecordWritableHost repoHost)
        {
            foreach(ActionElement ae in config.Elements)
            {
                IRecordWritableRepository repo = repoHost.CreateRecordWritableRepository(null, Current.CustomFilterServiceConfig.OutputFolder, ae.GroupName);
                actions.Add(ActionFactory.GetAction(ae, repo));
            }
        }
        #endregion Constructor ------------------------------------------------

        #region Public Methods ------------------------------------------------
        /// <summary>
        /// Change the output destinations based on the input source's name
        /// </summary>
        /// <param name="inputFileName">The name of the input source, for example, the input file's name without path</param>
        public void ChangeOutput(String inputName, String inputCustomer, String inputRecordType)
        {
            IEnumerable<Actions.Action> filteredActions = actions.Where(x => x.DoesActionApplyToInput(inputCustomer, inputRecordType));

            foreach (Actions.Action act in filteredActions)
            {
                act.ChangeOutput(inputName);
            }
        }
        
        /// <summary>
        /// Process all the passed Records through the configured Actions and output them to the appropriate destination
        /// </summary>
        /// <param name="records">An enumarable collection of Records representing the contents of an input file</param>
        public void ProcessActions(IEnumerable<Record> records, String inputCustomer, String inputRecordType)
        {
            IEnumerable<Actions.Action> filteredActions = actions.Where(x => x.DoesActionApplyToInput(inputCustomer, inputRecordType));

            foreach (Record record in records)
            {
                foreach (Actions.Action act in filteredActions)
                {
                    act.ProcessAction(record);
                }
            }
        }

        /// <summary>
        /// Cleanup the resources used by this object when it is no longer needed
        /// </summary>
        public void Dispose()
        {
            for(int i = actions.Count -1; i >= 0; i--)
            {
                actions[i].Dispose();
            }
        }
        #endregion Public Methods ---------------------------------------------
    }
}
