using System;
using DCL.RecordFilterService.Domain.Entities;

namespace DCL.CustomFilterService.Logic.Conditions
{
    /// <summary>
    /// A condition that is always met.  While not necessarily practical, it is useful for testing.
    /// </summary>
    public class AllInclusiveCondition : ICondition
    {
        #region Implemented Interface Methods ---------------------------------
        /// <summary>
        /// Does the configured field contain an allowed value for the passed Record
        /// </summary>
        /// <param name="record">The record to check</param>
        public bool IsMet(Record record)
        {
            return true;
        }

        /// <summary>
        /// Reset any history stored by this condition
        /// </summary>
        public void Reset()
        {
        }
        #endregion Implemented Interface Methods ------------------------------
    }
}