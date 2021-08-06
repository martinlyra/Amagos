using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amagos
{
    class Configuration
    {
        private readonly List<string> requiredFiles;

        internal Configuration()
        {
            requiredFiles = new List<string>();
        }

        internal Configuration Require(string configFilePath)
        {
            requiredFiles.Add(configFilePath);
            return this;
        } 

        internal bool CheckFiles()
        {
            List<string> missingFiles = new List<string>();
            foreach (string file in requiredFiles)
            {
                if (!File.Exists(file))
                    missingFiles.Add(file);
            }
            if (missingFiles.Count > 0)
            {
                throw new FileNotFoundException(
                    $"These files are required but are missing!\n{string.Join("\n\t -- ", missingFiles)}"
                    );
            }

            return true;
        }
    }
}
