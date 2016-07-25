using DCL.RecordFilterService.Configuration.Elements;
using System;
using System.Configuration;

namespace DCL.RecordFilterService.Configuration
{
    /// <summary>
    /// Represent the custom section of the config file that contains the settings specific to the CustomFilterService
    /// </summary>
    /// <example>
    /// <![CDATA[
    /// <configuration>             _____
    ///   <customFilterService ... >     |
    ///     ...                          |-- This class corresponds to this portion of the configuration, and its contents
    ///   </customFilterService>    _____|
    /// </configuration>
    /// ]]>
    /// </example>
    public class CustomFilterServiceSection : ConfigurationSection
    {
        /// <summary>
        /// Access the customFilterService node's inFolder attribute within the config file.
        /// This is used to specify where the input files are located.
        /// </summary>
        [ConfigurationProperty("inputFolder", IsRequired = true)]
        public string InputFolder
        {
            get
            {
                return this["inputFolder"] as string;
            }
            set
            {
                this["inputFolder"] = value;
            }
        }

        /// <summary>
        /// Access the customFilterService node's outputFolder attribute within the config file.
        /// This is used to specify where all output should be written to.
        /// </summary>
        [ConfigurationProperty("outputFolder", IsRequired = true)]
        public string OutputFolder
        {
            get
            {
                return this["outputFolder"] as string;
            }
            set
            {
                this["outputFolder"] = value;
            }
        }

        /// <summary>
        /// Access the customFilterService node's logFolder attribute within the config file.
        /// This is used to specify where all error logs should be written to.
        /// </summary>
        [ConfigurationProperty("logFolder", IsRequired = true)]
        public string LogFolder
        {
            get
            {
                return this["logFolder"] as string;
            }
            set
            {
                this["logFolder"] = value;
            }
        }

        /// <summary>
        /// Retrieves the custom collection of ActionElements, or action nodes, hosted within this configuration section.
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
        [ConfigurationProperty("", IsDefaultCollection = true)]
        public ActionElementCollection Elements
        {
            get { return (ActionElementCollection)base[""]; }
        }

        public override string ToString()
        {
            string openTag = "<customFilterService inputFolder='" + InputFolder + "' outputFolder='" + OutputFolder + "' logFolder='" + LogFolder + "' >";
            string closeTag = "</customFilterService>";

            return openTag + Environment.NewLine +
                Elements.ToString() +
                closeTag;
        }
    }
}
