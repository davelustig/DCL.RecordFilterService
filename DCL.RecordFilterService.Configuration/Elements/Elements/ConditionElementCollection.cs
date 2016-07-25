using System;
using System.Configuration;
using System.Text;

namespace DCL.RecordFilterService.Configuration.Elements
{
    /// <summary>
    /// Represent the custom collection of ConditionElements, or condition nodes, used to configure to the CustomFilterService.
    /// These are usually hosted within a custom action section in the config file.
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
    public class ConditionElementCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Identifies the type of this ConfigurationElement collection.  There are basically two types:
        /// AddRemoveClearMap, which allows child config files to modify this collection using add, remove,
        /// and clear elements; and BasicMap, which does not allow modifications from child config files.
        /// This collection is a BasicMap.
        /// </summary>
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        /// <summary>
        /// The name of the configuration node(s) hosted within this collection
        /// </summary>
        protected override string ElementName
        {
            get { return "condition"; }
        }

        /// <summary>
        /// Create a new ConditionElement
        /// </summary>
        protected override ConfigurationElement CreateNewElement()
        {
            return new ConditionElement();
        }

        /// <summary>
        /// Obtains a unique key used to identify a specific ConditionElement within this collection
        /// </summary>
        /// <param name="element">The ConditionElement to retrieve a key for/from</param>
        /// <returns></returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            ConditionElement ce = (ConditionElement)element;
            if (ce.Type == ConditionType.isAllowed)
                return string.Concat(ce.Type.ToString(), "|", ce.Field, "|", ce.Value);
            else if (ce.Type == ConditionType.isDuplicate)
                return string.Concat(ce.Type.ToString(), "|", ce.Field);
            else if (ce.Type == ConditionType.isInRange)
                return string.Concat(ce.Type.ToString(), "|", ce.Field, "|", ce.RangeStart, "|", ce.RangeEnd);

            return string.Concat(ce.Type.ToString(), "|", ce.Field);
        }

        /// <summary>
        /// Add a ConditionElement to the ConditionElementCollection
        /// </summary>
        /// <param name="ce">The ConditionElement to add</param>
        public void Add(ConditionElement ce)
        {
            BaseAdd(ce);
        }

        /// <summary>
        /// Remove a ConditionElement from the ConditionElementCollection
        /// </summary>
        /// <param name="ce">The ConditionElement to remove</param>
        public void Remove(ConditionElement ce)
        {
            BaseRemove(GetElementKey(ce) as String);
        }

        public override string ToString()
        {
            StringBuilder contents = new StringBuilder();

            foreach (ConditionElement ce in this)
            {
                contents.Append(ce.ToString());
                contents.Append(Environment.NewLine);
            }

            return contents.ToString();
        }
    }
}
