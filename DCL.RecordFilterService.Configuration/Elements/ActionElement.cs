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
    public enum ActionType
    {
        remove = 0,
        group = 1
    }

    /// <summary>
    /// Represent an ActionElement, or action node, used to configure to the CustomFilterService.
    /// One or more of these are usually hosted within the custom customFilterService section in the config file.
    /// </summary>
    /// <example>
    /// <![CDATA[
    /// <configuration>
    ///   <customFilterService ... > _____
    ///     <action ... >                 |
    ///       <condition ... />           |-- This class corresponds to this portion of the configuration, and its contents
    ///       ...                         |
    ///     </action>                _____|
    ///     <action ... >
    ///       ...
    ///     </action>
    ///   </customFilterService>
    /// </configuration>
    /// ]]>
    /// </example>
    public class ActionElement : ConfigurationElement
    {
        /// <summary>
        /// Access the ActionElement's type attributed, which is used to specify the action to take
        /// </summary>
        [ConfigurationProperty("type", IsRequired = true)]
        public ActionType @Type
        {
            get
            {
                return (ActionType)this["type"];
            }
            set
            {
                this["type"] = value;
            }
        }

        /// <summary>
        /// Access the ActionElement's groupName attributes, which is used to specify the name of the group to sort accounts into
        /// </summary>
        [ConfigurationProperty("groupName", DefaultValue = "Default")]
        public string GroupName
        {
            get
            {
                return this["groupName"] as string;
            }
            set
            {
                this["groupName"] = value;
            }
        }

        /// <summary>
        /// Access the ActionElement's customer attributes, which is used to specify the customer whose input files should be processed by this action
        /// </summary>
        [ConfigurationProperty("customer", DefaultValue = "")]
        public string Customer
        {
            get
            {
                return this["customer"] as string;
            }
            set
            {
                this["customer"] = value;
            }
        }

        /// <summary>
        /// Access the ActionElement's inputRecordType attributes, which is used to specify the file record classification of the files to be processed by this action
        /// </summary>
        [ConfigurationProperty("inputRecordType", DefaultValue = "")]
        public string InputRecordType
        {
            get
            {
                return this["inputRecordType"] as string;
            }
            set
            {
                this["inputRecordType"] = value;
            }
        }

        /// <summary>
        /// Retrieves the custom collection of ConditionElements, or condition nodes, hosted within this configuration element
        /// </summary>
        /// <example>
        /// <![CDATA[
        /// <configuration>
        ///   <customFilterService ... > 
        ///     <action ... >       _____
        ///       <condition ... />      |
        ///       <condition ... /> _____|-- This class corresponds to this portion of the configuration, and its contents
        ///     </action>
        ///   </customFilterService>
        /// </configuration>
        /// ]]>
        /// </example>
        [ConfigurationProperty("", IsDefaultCollection = true)]
        public ConditionElementCollection Elements
        {
            get { return (ConditionElementCollection)base[""]; }
        }

        public override string ToString()
        {
            StringBuilder openTag = new StringBuilder();
            openTag.Append("<action type='" + this.Type + "'");
            if (Type == ActionType.group)
                openTag.Append(" groupName='" + GroupName + "'");
            if (String.IsNullOrEmpty(Customer) == false)
                openTag.Append(" customer='" + Customer + "'");
            if (String.IsNullOrEmpty(InputRecordType) == false)
                openTag.Append(" inputRecordType='" + InputRecordType + "'");
            openTag.Append(">");

            string closeTag = "</action>";

            return openTag.ToString() + Environment.NewLine +
                Elements.ToString() +
                closeTag;
        }
    }
}
