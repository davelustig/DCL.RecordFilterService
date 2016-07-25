using System;
using System.Linq;
using DCL.RecordFilterService.Domain.Entities;

namespace DCL.CustomFilterService.Logic.Conditions
{
    public class RangedCondition : ICondition
    {
        #region Private Members -----------------------------------------------
        private string field;
        private long rangeStart;
        private long rangeEnd;
        #endregion Private Members --------------------------------------------

        #region Constructor ---------------------------------------------------
        /// <summary>
        /// Create a new Ranged condition that will determine whether the specified field is within the given range, inclusive, for a Record
        /// </summary>
        /// <param name="field">The name of the field to check</param>
        /// <param name="rangeStart">The beginning of the range, inclusive</param>
        /// <param name="rangeEnd">The end of the range, inclusive</param>
        public RangedCondition(string field, long rangeStart, long rangeEnd)
        {
            if (string.IsNullOrEmpty(field))
                throw new ArgumentNullException("field");

            this.field = field;
            this.rangeStart = rangeStart;
            this.rangeEnd = rangeEnd;
        }
        #endregion Constructor ------------------------------------------------

        #region Implemented Interface Methods ---------------------------------
        /// <summary>
        /// Is the configured field within the desired range for the passed Record
        /// </summary>
        /// <param name="record">The record to check</param>
        public bool IsMet(Record record)
        {
            if (record.Contains(field))
            {
                // Attempt to convert the field value into an number
                long valueAsNumber;
                bool valueCanBeConverted = long.TryParse(record[field].ToString(), out valueAsNumber);
                if (valueCanBeConverted == false)
                {
                    string digitsInValue = new string(record[field].ToString().Where(c => Char.IsDigit(c)).ToArray());
                    valueCanBeConverted = long.TryParse(digitsInValue, out valueAsNumber);
                }

                if(valueCanBeConverted == true)
                    return (rangeStart <= valueAsNumber && valueAsNumber <= rangeEnd);
            }

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
