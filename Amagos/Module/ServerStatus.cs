using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Amagos.Module
{
    struct ServerStatusData
    {
        public string Version;
        public string Mode;
        public string PlayerCount;
        public string RoundDuration;
    }

    static class ServerStatus
    {
        internal async static Task<ServerStatusData> FetchServerData()
        {
            try
            {
                var web = new WebClient();
                string stream = await web.DownloadStringTaskAsync("http://baystation12.net/status.dat");
                var json = JObject.Parse(stream);

                var statusdata = new ServerStatusData();

                statusdata.Version = json.SelectToken("version").ToString();
                statusdata.Mode = json.SelectToken("mode").ToString();
                statusdata.PlayerCount = json.SelectToken("players").ToString();
                statusdata.RoundDuration = json.SelectToken("roundduration").ToString();

                return statusdata;
            }
            catch
            {
                return new ServerStatusData();
            }
        }
    }
}
