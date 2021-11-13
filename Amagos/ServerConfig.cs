using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amagos
{
    public class ServerConfig
    {
        public List<ulong> PublicCommandChannels = new List<ulong>();
        public Dictionary<Type, ModuleConfig> ModuleConfigs = new Dictionary<Type, ModuleConfig>();

        public T fetchModuleConfig<T> () where T : ModuleConfig
        {
            ModuleConfig value = null;
            if(!ModuleConfigs.TryGetValue(typeof(T), out value))
            {
                value = new ModuleConfig();
                ModuleConfigs[typeof(T)] = value;
            }
            return (T)value;
        }
    }
}
