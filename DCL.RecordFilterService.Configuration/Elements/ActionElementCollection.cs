using System;
using System.Configuration;
using System.Text;

namespace DCL.RecordFilterService.Configuration.Elements
{
    /// <summary>
    /// Represent the custom collection of ActionElements, or action nodes, used to configure to the CustomFilterService.
    /// These are usually hosted within the custom customFilterService section in the config file.
    /// </summary>
    /// <example>
    /// <![CDATA[
    /// <configuration>
    ///   <customFilterService ... > _____
    ///     <action ... >                 |
    ///       ...                         |
    ///     </action>                     |-- This class corresponds to this portion of the configuration, and its contents
    ///     <action ... >                 |
    ///       ...                         |
    ///     </action>                _____|
    ///   </customFilterService>
    /// </configuration>
    /// ]]>
    /// </example>
    public class ActionElementCollection : ConfigurationElementCollection
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
            get { return "action"; }
        }

        /// <summary>
        /// Create a new ActionElement
        /// </summary>
        protected override ConfigurationElement CreateNewElement()
        {
            return new ActionElement();
        }

        /// <summary>
        /// Obtains a unique key used to identify a specific ActionElement within this collection
        /// </summary>
        /// <param name="element">The ActionElement to retrieve a key for/from</param>
        /// <returns></returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            ActionElement ae = (ActionElement)element;
            return ae.Type.ToString() + ae.GroupName;
        }


        public override string ToString()
        {
            StringBuilder contents = new StringBuilder();

            foreach(ActionElement ae in this)
            {
                contents.Append(ae.ToString());
                contents.Append(Environment.NewLine);
            }

            return contents.ToString();
        }
    }
}
