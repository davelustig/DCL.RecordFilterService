using System;
using System.Collections.Generic;
using System.Text;
using DCL.RecordFilterService.Domain.Entities;

namespace DCL.CustomFilterService.Logic.Conditions
{
    public class DuplicateCondition : ICondition
    {
        #region Private Members -----------------------------------------------
        private string[] fields;
        private Dictionary<string, int> duplicateHistory = new Dictionary<string, int>();
        #endregion Private Members --------------------------------------------

        #region Constructor ---------------------------------------------------
        /// <summary>
        /// Create a new Duplicate condition that will identify duplicate Records based on the specified fields
        /// </summary>
        /// <param name="duplicateFields">A comma-delimited string of the CSV fields/Record properties to use when identifying duplicates</param>
        public DuplicateCondition(string duplicateFields)
        {
            if (string.IsNullOrEmpty(duplicateFields))
                throw new ArgumentNullException("duplicateFields");

            fields = duplicateFields.Split(new char[] { ',' });
        }
        #endregion Constructor ------------------------------------------------

        #region Implemented Interface Methods ---------------------------------
        /// <summary>
        /// Is this record a duplicate of a previously processed record
        /// </summary>
        /// <param name="record">The record to check</param>
        public bool IsMet(Record record)
        {
            string key = BuildKey(record);

            if (duplicateHistory.ContainsKey(key))
            {
                duplicateHistory[key]++;
                return true;
            }
            else
            {
                duplicateHistory.Add(key, 1);
                return false;
            }
        }

        /// <summary>
        /// Reset any history stored by of this condition
        /// </summary>
        public void Reset()
        {
            duplicateHistory.Clear();
        }
        #endregion Implemented Interface Methods ------------------------------

        #region Helper Methods ------------------------------------------------
        /// <summary>
        /// Build a unique key for the passed record based on the fields specified to be monitored for duplicates.
        /// This key will be used to identify whether two records are duplicates.
        /// </summary>
        /// <param name="record">The record to build a key for</param>
        private string BuildKey(Record record)
        {
            StringBuilder keySB = new StringBuilder();

            foreach (string field in fields)
            {
                if (record.Contains(field) == true)
                    keySB.Append(record[field] + "|");
            }

            return keySB.ToString();
        }
        #endregion Helper Methods ---------------------------------------------
    }
}
