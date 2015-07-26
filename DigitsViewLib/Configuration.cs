using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Diagnostics;


namespace DigitsViewLib
{
    public class Configuration
    {
        System.Configuration.Configuration theConfig;

        public Configuration(string configFilename)
        {
            ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
            configMap.ExeConfigFilename = configFilename;
            theConfig = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);

        }

        public string this[string aKey]
        {
           get
           {
               return theConfig.AppSettings.Settings[aKey].Value;
           }
        }

/*        public int this[string aKey]
        {
            get
            {
                return Int64.Parse(theConfig.AppSettings.Settings[aKey].Value);
            }
        }
 * */
    }
}
