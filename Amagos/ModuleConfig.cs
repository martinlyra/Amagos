using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Amagos
{
    public class ModuleConfig
    {
        private readonly IDictionary<string, object> config;

        public ModuleConfig()
        {
            config = new Dictionary<string, object>();
        }

        public T getConfigEntry<T>(string key, T defaultValue)
        {
            object value;
            if (config.TryGetValue(key, out value))
                return (T)value;
            else
                return defaultValue;
        }

        public void setConfigEntry<T>(string key, T value)
        {
            config[key] = value;
        }
    }
}
