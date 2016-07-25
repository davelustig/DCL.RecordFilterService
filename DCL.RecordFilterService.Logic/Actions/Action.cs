using DCL.RecordFilterService.Configuration.Elements;
using DCL.RecordFilterService.Domain.Entities;
using DCL.CustomFilterService.Logic.Conditions;
using System;
using System.Collections.Generic;

namespace DCL.CustomFilterService.Logic.Actions
{
    public abstract class Action: IDisposable
    {
        #region Members -------------------------------------------------------
        public List<String> Customers = new List<string>();
        public List<String> InputRecordTypes = new List<string>();
        protected List<ICondition> conditions = new List<ICondition>();
        #endregion Members ----------------------------------------------------

        #region Constructor ---------------------------------------------------
        /// <summary>
        /// Create a basic action with associated criteria
        /// </summary>
        /// <param name="ae">The action's configuration information</param>
        /// <remarks>This will need to be subclassed to implement any special logic to be executed when all criteria are met</remarks>
        public Action (ActionElement ae)
        {
            if (String.IsNullOrEmpty(ae.Customer) == false)
                Customers.AddRange(ae.Customer.Split(','));
            if (String.IsNullOrEmpty(ae.InputRecordType) == false)
                InputRecordTypes.AddRange(ae.InputRecordType.Split(','));

            foreach (ConditionElement ce in ae.Elements)
            {
                conditions.Add(ConditionFactory.GetCondition(ce));
            }
        }
        #endregion Constructor ------------------------------------------------

        #region Abstract Methods - Definitions and Declarations ---------------
        /// <summary>
        /// Determine if this action should be applied to the input of a specific customer and record type
        /// </summary>
        /// <param name="inputCustomer">The customer the input record is from</param>
        /// <param name="inputRecordType">The type of the input record</param>
        /// <returns></returns>
        public bool DoesActionApplyToInput(String inputCustomer, String inputRecordType)
        {
            bool doesActionAppy;

            // If this action is not restricted to specific customers, or if it is and the passed customer is one of those
            doesActionAppy = (Customers.Count == 0) ? true : Customers.Contains(inputCustomer);

            // If this action is not restricted to specific record types, or if it is and the passed record type is one
            doesActionAppy = doesActionAppy && ((InputRecordTypes.Count == 0) ? true : InputRecordTypes.Contains(inputRecordType));

            return doesActionAppy;
        }
        
        /// <summary>
        /// Check if the passed record meets all the conditions associated with this action
        /// </summary>
        /// <param name="record">The Record to check</param>
        public bool AreConditionsMet(Record record)
        {
            bool areConditionsMet = true;

            foreach (ICondition condition in conditions)
            {
                areConditionsMet = areConditionsMet && condition.IsMet(record);
            }

            return areConditionsMet;
        }

        public abstract void ProcessAction(Record record);
        public abstract void ProcessAction(Record record, String inputCustomer, String inputRecordType);
        public abstract void ChangeOutput(String inputName);

        public abstract void Dispose();
        #endregion Abstract Methods - Definitions and Declarations ------------
    }
}
