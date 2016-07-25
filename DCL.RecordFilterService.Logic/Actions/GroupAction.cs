using DCL.RecordFilterService.Configuration.Elements;
using DCL.RecordFilterService.Domain.Abstract;
using DCL.RecordFilterService.Domain.Entities;
using DCL.CustomFilterService.Logic.Conditions;
using System;

namespace DCL.CustomFilterService.Logic.Actions
{
    public class GroupAction: Action
    {
        #region Private Members -----------------------------------------------
        /// <summary>The name of the output group associated with this action</summary>
        /// <remarks>This is not currently used, since the group-name logic is specific to the WritableRepository.  It's included here for future usage, if needed.</remarks>
        private string groupName;

        /// <summary>The output handler responsible for writing Records to an output destination</summary>
        private IRecordWritableRepository outputRepo;
        #endregion Private Members --------------------------------------------

        #region Constructors --------------------------------------------------
        /// <summary>
        /// Create a new Grouping Action, which will group Records together into one output file if they meet the specified criteria.
        /// </summary>
        /// <param name="ae">The group action's configuration information</param>
        /// <param name="outputRepo">The file output repository used to write the output file</param>
        public GroupAction(ActionElement ae, IRecordWritableRepository outputRepo): base (ae)
        {
            this.groupName = ae.GroupName;

            this.outputRepo = outputRepo;
        }
        #endregion Constructors -----------------------------------------------

        #region Public Methods ------------------------------------------------
        /// <summary>
        /// Check if the passed Record meets all conditions specified for this action.  
        /// If so, write the Record to the correct output.
        /// </summary>
        /// <param name="record">The record to check</param>
        public override void ProcessAction(Record record)
        {
            if (AreConditionsMet(record))
                outputRepo.AddRecordToOutput(record);
        }

        /// <summary>
        /// Check if the passed Record meets all conditions specified for this action.  
        /// If so, write the Record to the correct output.
        /// </summary>
        /// <param name="record">The record to check</param>
        /// <param name="inputCustomer">The customer the input record is from</param>
        /// <param name="inputRecordType">The type of the input record</param>
        public override void ProcessAction(Record record, String inputCustomer, String inputRecordType)
        {
            if (DoesActionApplyToInput(inputCustomer, inputRecordType) && AreConditionsMet(record))
                outputRepo.AddRecordToOutput(record);
        }

        /// <summary>
        /// Change the output destination based on the input source's name
        /// </summary>
        /// <param name="inputFileName">The name of the input source, for example, the input file's name without path</param>
        public override void ChangeOutput(String inputFileName)
        {
            outputRepo.ChangeOutputDestination(inputFileName);

            // Reset any input-specific settings, history, cache, etc for each condition
            foreach (ICondition condition in conditions)
                condition.Reset();
        }

        /// <summary>
        /// Cleanup the resources used by this object when it is no longer needed
        /// </summary>
        public override void Dispose()
        {
            outputRepo.Dispose();
        }
        #endregion Public Methods ---------------------------------------------
    }
}
