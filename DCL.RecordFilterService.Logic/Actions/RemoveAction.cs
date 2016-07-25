using DCL.RecordFilterService.Configuration.Elements;
using DCL.RecordFilterService.Domain.Abstract;
using DCL.RecordFilterService.Domain.Entities;
using DCL.CustomFilterService.Logic.Conditions;
using System;

namespace DCL.CustomFilterService.Logic.Actions
{
    public class RemoveAction: Action
    {
        #region Private Members -----------------------------------------------
        /// <summary>The output handler responsible for writing Records to an output destination</summary>
        private IRecordWritableRepository outputRepo;
        #endregion Private Members --------------------------------------------

        #region Constructors --------------------------------------------------
        /// <summary>
        /// Create a new Remove Record Action, which will group only the Records together that do not meet the specified criteria.
        /// </summary>
        /// <param name="ae">The remove action's configuration information</param>
        /// <param name="outputRepo">The file output repository used to write the output file</param>
        public RemoveAction(ActionElement ae, IRecordWritableRepository outputRepo): base (ae)
        {
            this.outputRepo = outputRepo;
        }
        #endregion Constructors -----------------------------------------------

        #region Public Methods ------------------------------------------------
        /// <summary>
        /// Check if the passed Record meets all conditions specified for this action.  
        /// If not, write the Record to the correct output.
        /// </summary>
        /// <param name="record">The record to check</param>
        public override void ProcessAction(Record record)
        {
            if (AreConditionsMet(record) == false)
                outputRepo.AddRecordToOutput(record);
        }

        /// <summary>
        /// Check if the passed Record meets all conditions specified for this action.  
        /// If not, write the Record to the correct output.
        /// </summary>
        /// <param name="record">The record to check</param>
        /// <param name="inputCustomer">The customer the input record is from</param>
        /// <param name="inputRecordType">The type of the input record</param>
        public override void ProcessAction(Record record, String inputCustomer, String inputRecordType)
        {
            if (DoesActionApplyToInput(inputCustomer, inputRecordType) && AreConditionsMet(record) == false)
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
