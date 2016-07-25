using System;
using System.Linq;
using DCL.RecordFilterService.Domain.Entities;

namespace DCL.CustomFilterService.Logic.Conditions
{
    public class AllowedCondition : ICondition
    {
        #region Private Members -----------------------------------------------
        private string field;
        private string[] allowedValues;
        #endregion Private Members --------------------------------------------

        #region Constructor ---------------------------------------------------
        /// <summary>
        /// Create a new Allowed Condition that will check if a specified field within a Record contains an allowed value
        /// </summary>
        /// <param name="field">The name of the field to check</param>
        /// <param name="allowedValues">A comma-delimited list of valid values for the field</param>
        public AllowedCondition(string field, string allowedValues)
        {
            if (string.IsNullOrEmpty(field))
                throw new ArgumentNullException("field");

            if (string.IsNullOrEmpty(allowedValues))
                throw new ArgumentNullException("allowedValues");

            this.field = field;
            this.allowedValues = allowedValues.Split(new char[] { ',' });
        }
        #endregion Constructor ------------------------------------------------

        #region Implemented Interface Methods ---------------------------------
        /// <summary>
        /// Does the configured field contain an allowed value for the passed Record
        /// </summary>
        /// <param name="record">The record to check</param>
        public bool IsMet(Record record)
        {
            if (record.Contains(field))
                return allowedValues.Contains(record[field]);
            else
                return false;
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
