using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Razer.Common
{
    /// <summary>
    /// Custom configuration section for mapping the windows to menu items
    /// </summary>
    public class WindowMappingElement : ConfigurationElement
    {
        [ConfigurationProperty("buttonId", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string ButtonId
        {
            get
            {
                return ((string)(base["buttonId"]));
            }
            set
            {
                base["buttonId"] = value;
            }
        }

        [ConfigurationProperty("razerObject", DefaultValue = "", IsKey = false, IsRequired = true)]
        public string RazerObject
        {
            get
            {
                return ((string)(base["razerObject"]));
            }
            set
            {
                base["razerObject"] = value;
            }
        }

        [ConfigurationProperty("windowType", DefaultValue = "", IsKey = false, IsRequired = true)]
        public string WindowType
        {
            get
            {
                return ((string)(base["windowType"]));
            }
            set
            {
                base["windowType"] = value;
            }
        }

        [ConfigurationProperty("isDialog", DefaultValue = false, IsKey = false, IsRequired = true)]
        public bool IsDialog
        {
            get
            {
                return ((bool)(base["isDialog"]));
            }
            set
            {
                base["isDialog"] = value;
            }
        }   

        [ConfigurationProperty("windowCaption", DefaultValue = "", IsKey = false, IsRequired = false)]
        public string WindowCaption
        {
            get
            {
                return ((string)(base["windowCaption"]));
            }
            set
            {
                base["windowCaption"] = value;
            }
        }
        
        /// <summary>
        /// Use to store the parameter name passed to an open window so the correct data can be loaded.
        /// </summary>
        [ConfigurationProperty("parameter", DefaultValue = "", IsKey = false, IsRequired = false)]
        public string ParameterId
        {
            get
            {
                return ((string)(base["parameter"]));
            }
            set
            {
                base["parameter"] = value;
            }
        }

        /// <summary>
        /// Stores the control id that will host this screen.  Used when the screen is a 
        /// part of the content of another screen that is already loaded.
        /// </summary>
        [ConfigurationProperty("contentId", DefaultValue = "", IsKey = false, IsRequired = false)]
        public string ContentId
        {
            get
            {
                return ((string)(base["contentId"]));
            }
            set
            {
                base["contentId"] = value;
            }
        }


        [ConfigurationProperty("parameters", IsRequired = false)]       
        public ParameterCollection ParameterList
        {
            get
            {
                return ((ParameterCollection)(base["parameters"]));
            }           
        }  
    }


    /// <summary>
    /// Element to hold parmater information for the window
    /// </summary>
    public class ParameterElement : ConfigurationElement
    {
        [ConfigurationProperty("name", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string Name
        {
            get
            {
                return ((string)(base["name"]));
            }
            set
            {
                base["name"] = value;
            }
        }
    }


    /// <summary>
    /// Need a collection since there are multiple entries.
    /// </summary>
    [ConfigurationCollection(typeof(WindowMappingElement))]
    public class WindowMappingCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new WindowMappingElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((WindowMappingElement)(element)).ButtonId;
        }

        public WindowMappingElement this[int idx]
        {
            get
            {
                return (WindowMappingElement)BaseGet(idx);
            }
        }

        public WindowMappingElement this[string key]
        {
            get
            {
                return (WindowMappingElement)BaseGet(key);
            }
        }
    }

    /// Need a collection since there are multiple entries.
    /// </summary>
    [ConfigurationCollection(typeof(ParameterElement))]
    public class ParameterCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ParameterElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ParameterElement)(element)).Name;
        }

        public ParameterElement this[int idx]
        {
            get
            {
                return (ParameterElement)BaseGet(idx);
            }
        }

        public ParameterElement this[string key]
        {
            get
            {
                return (ParameterElement)BaseGet(key);
            }
        }
    }

    /// <summary>
    /// Represents the custom section defined in <configsections></configsections>
    /// </summary>
    public class WindowMappingsConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("WindowMappings")]
        public WindowMappingCollection MappingItems
        {
            get { return ((WindowMappingCollection)(base["WindowMappings"])); }
        }
    }    
}
