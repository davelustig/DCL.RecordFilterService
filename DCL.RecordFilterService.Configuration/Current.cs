using System;
using System.Configuration;

namespace DCL.RecordFilterService.Configuration
{
    public static class Current
    {
        #region Configuration Accessors -----------------------------------------------------------
        private static CustomFilterServiceSection _customFilterServiceConfig = null;

        /// <summary>
        /// Object representing the entire section of configuration information for the CustomFilterService.
        /// </summary>
        public static CustomFilterServiceSection CustomFilterServiceConfig
        {
            get
            {
                if(_customFilterServiceConfig == null)
                {
                    _customFilterServiceConfig = (CustomFilterServiceSection)ConfigurationManager.GetSection("DCL/customFilterService");

                    //LogConfiguration();
                }
                return _customFilterServiceConfig;
            }
        }
        #endregion Configuration Accessors --------------------------------------------------------
    }
}
