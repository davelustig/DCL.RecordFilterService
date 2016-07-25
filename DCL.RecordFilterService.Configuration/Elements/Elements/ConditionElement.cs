using System;
using System.Configuration;
using System.Text;

namespace DCL.RecordFilterService.Configuration.Elements
{
    /// <summary>
    /// The enumeration used by the ActionElement to specify what type 
    /// of action should be taken if the associated conditions are met
    /// </summary>
    [Flags]
    public enum ConditionType
    {
        isDuplicate = 0,
        isInRange = 1,
        isAllowed = 2,
        allInclusive = 3
    }

    /// <summary>
    /// Represent a ConditionElement, or condition node, used to configure to the CustomFilterService.
    /// These are usually hosted within a custom action section in the config file.
    /// </summary>
    /// <example>
    /// <![CDATA[
    /// <configuration>
    ///   <customFilterService ... > 
    ///     <action ... >       _____
    ///       <condition ... /> _____|-- This class corresponds to this portion of the configuration, and its contents
    ///       <condition ... /> 
    ///     </action>
    ///   </customFilterService>
    /// </configuration>
    /// ]]>
    /// </example>
    public class ConditionElement : ConfigurationElement
    {
        /// <summary>
        /// Access the ConditionElement's type attributed, which is used to specify the condition type
        /// </summary>
        [ConfigurationProperty("type", IsRequired = true)]
        public ConditionType @Type
        {
            get
            {
                return (ConditionType)this["type"];
            }
            set
            {
                this["type"] = value;
            }
        }

        /// <summary>
        /// Access the ConditionElement's field attributed, which is used to specify which fields are to be examined for the condition
        /// </summary>
        [ConfigurationProperty("field", IsRequired = true)]
        public string Field
        {
            get
            {
                return this["field"] as string;
            }
            set
            {
                this["field"] = value;
            }
        }

        /// <summary>
        /// Access the ConditionElement's value attributed, which is used to specify the desired value of a field when the Type is isAllowed
        /// </summary>
        [ConfigurationProperty("value", IsRequired = false)]
        public string Value
        {
            get
            {
                return this["value"] as string;
            }
            set
            {
                this["value"] = value;
            }
        }

        /// <summary>
        /// Access the ConditionElement's rangeStart attributed, which is used to specify the starting value of a valid range when the Type is isInRange
        /// </summary>
        [ConfigurationProperty("rangeStart", IsRequired = false)]
        public int RangeStart
        {
            get
            {
                return (int)this["rangeStart"];
            }
            set
            {
                this["rangeStart"] = value;
            }
        }

        /// <summary>
        /// Access the ConditionElement's rangeEnd attributed, which is used to specify the ending value of a valid range when the Type is isInRange
        /// </summary>
        [ConfigurationProperty("rangeEnd", IsRequired = false)]
        public int RangeEnd
        {
            get
            {
                return (int)this["rangeEnd"];
            }
            set
            {
                this["rangeEnd"] = value;
            }
        }

        public override string ToString()
        {
            StringBuilder tag = new StringBuilder();
            tag.Append("<condition type='" + Type + "' field='" + Field + "'");

            switch (Type)
            {
                case ConditionType.isAllowed: 
                    tag.Append(" value='" + Value + "'");
                    break;
                case ConditionType.isDuplicate:
                    break;
                case ConditionType.isInRange:
                    tag.Append(" rangeStart='" + RangeStart + " rangeEnd='" + RangeEnd + "'");
                    break;
                default:
                    break;
            }
            tag.Append(" />");

            return tag.ToString();
        }
    }
}
