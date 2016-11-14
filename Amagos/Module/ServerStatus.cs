using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Amagos.Module
{
    struct ServerStatusData
    {
        public string Version;
        public string Mode;
        public string PlayerCount;
        public string RoundDuration;

        public TimeSpan RoundDurationAsTimeSpan()
        {
            var s = Uri.UnescapeDataString(RoundDuration).Split(':');
            return new TimeSpan(int.Parse(s[0]), int.Parse(s[1]), 0);
        }
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

                Console.WriteLine(Uri.UnescapeDataString(statusdata.RoundDuration));

                return statusdata;
            }
            catch
            {
                return new ServerStatusData();
            }
        }

        internal async static Task<IEnumerable<string>> FetchPlayerData()
        {
            IEnumerable<string> playerdata;
            try
            {
                var web = new WebClient();
                string stream = await web.DownloadStringTaskAsync("http://baystation12.net/status.dat");
                var json = JObject.Parse(stream);

                var rawdata = json.ToString();

                playerdata = rawdata.Replace('{', ' ').Replace('}', ' ').Split(',')
                    .Where(s => s.Contains("player") && !s.Contains("players"))
                    .Select(s => s.Split(':')[1].Replace('"', '\'').Replace('+', ' '))
                    .OrderBy(s => s);
                //.Where(s => !s.Contains("player"));
                //playerdata.ToList().ForEach(s => s = s.Split(':')[1]);
                Console.WriteLine(playerdata.ToString());

            } catch
            {
                return new List<string>();
            }

            return playerdata;
        }
    }
}
