using System;
using System.Collections.Specialized;
using System.Text;

namespace DCL.RecordFilterService.Domain.Entities
{
    public class Record: OrderedDictionary
    {
        #region Constructors --------------------------------------------------
        /// <summary>
        /// Create a new record object, intially empty
        /// </summary>
        /// <remarks>This constructor is primarily for testing</remarks>
        public Record()
        {
        }
        
        /// <summary>
        /// Create a new record object from a line of a CSV file
        /// </summary>
        /// <param name="csvLine">
        /// The line of a CSV file containing the values to populate this object from, in the following order: 
        /// [FirstName],[LastName],[PhoneNumber],[Age],[Gender]
        /// </param>
        public Record(string csvFields, string csvLine)
        {
            if (string.IsNullOrEmpty(csvFields))
                throw new ArgumentNullException("The CSV field collection is null or empty.  Empty field names are not supported.");
            if (string.IsNullOrEmpty(csvLine))
                throw new ArgumentNullException("The CSV line is null or empty.  Empty lines are not supported.");

            string[] fieldNames = csvFields.Split(new char[] { ',' }, StringSplitOptions.None);
            string[] fieldValues = csvLine.Split(new char[] { ',' }, StringSplitOptions.None);
            if (fieldNames.Length != fieldValues.Length)
                throw new ArgumentException("The CSV line does not contain all the specified fields.  It contains: '" + csvLine + "', but the fields specified were '" + csvFields + "'.");

            this.Clear();
            for(int i = 0; i < fieldNames.Length; i++)
            {
                this.Add(fieldNames[i], fieldValues[i]);
            }
        }
        #endregion Constructors -----------------------------------------------

        #region Public Methods ------------------------------------------------
        /// <summary>
        /// Construct a CSV string representation of this object
        /// </summary>
        /// <returns>A comma-delimited string, with the fields in the order they were added</returns>
        public string ToCsvString()
        {
            StringBuilder csvLineSB = new StringBuilder();

            for(int i = 0; i < this.Count; i++)
            {
                csvLineSB.Append(this[i]);

                if (i < this.Count - 1)
                    csvLineSB.Append(",");
            }

            return csvLineSB.ToString();
        }

        /// <summary>
        /// Construct a CSV string labeling the fields of this object
        /// </summary>
        public string CsvFieldNamesString()
        {
            StringBuilder csvFieldNamesSB = new StringBuilder();

            String[] keyNames = new String[this.Count];
            this.Keys.CopyTo(keyNames, 0);

            for (int i = 0; i < keyNames.Length; i++)
            {
                csvFieldNamesSB.Append(keyNames[i]);

                if (i < this.Count - 1)
                    csvFieldNamesSB.Append(",");
            }

            return csvFieldNamesSB.ToString();
        }
        #endregion Public Methods ---------------------------------------------
    }
}
