using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;

namespace Amagos.Module
{
    internal class ServerStatusModule : BotModule
    {
        public struct ServerStatusData
        {
            public string Version;
            public string Mode;
            public string PlayerCount;
            public string RoundDuration;
            public string Map;

            public TimeSpan RoundDurationAsTimeSpan()
            {
                var s = Uri.UnescapeDataString(RoundDuration).Split(':');
                return new TimeSpan(int.Parse(s[0]), int.Parse(s[1]), 0);
            }
        }

        public override async void OnCommandReceived(object sender, MessageEventArgs e)
        {
            var message = e.Message;

            if (message.Text.Contains("serverstatus"))
            {
                var data = await FetchServerData();
                if (data.Version == null)
                    await e.Channel.SendMessage("Failed to retrieve server data. Currently offline, perhaps?");
                else
                {
                    var timespan = data.RoundDurationAsTimeSpan();
                    Func<string> duration_string = () =>
                    {
                        var h = timespan.Hours;
                        var m = timespan.Minutes;
                        return
                            $"{(h > 0 ? $"{h} hour{(h > 1 ? 's' : '\0')}{(m > 0 ? " and " : "\0")}" : "\0")}"
                            + $"{(m > 0 ? $"{m} minute{(m > 1 ? 's' : '\0')}" : "\0")}";
                    };
                    var response =
                        $"**Server is up** - {data.Version}, with {data.PlayerCount} "
                        + $"players on the gamemode \'{data.Mode}\' at \'{data.Map}\'."
                        + $" Currently {duration_string()} in the game.";
                    Console.WriteLine(response);
                    await e.Channel.SendMessage(response);
                }
            }
            if (message.Text.Contains("players"))
            {
                var data = await FetchPlayerData();
                var count = data.Count();
                if (count == 0)
                    await e.Channel.SendMessage("Failed to retrieve server data. Currently offline, perhaps?");
                else
                {
                    var response = $"**Server is up** - {count} players present on server: ```";
                    data.ToList().ForEach(p => response += $"{p.Trim()} ");
                    response += "```";
                    Console.WriteLine(response);
                    await e.Channel.SendMessage(response);
                }
            }
        }

        internal async Task<ServerStatusData> FetchServerData()
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
                statusdata.Map = json.SelectToken("map").ToString().Replace('+', ' ');

                Console.WriteLine(Uri.UnescapeDataString(statusdata.RoundDuration));

                return statusdata;
            }
            catch
            {
                return new ServerStatusData();
            }
        }

        internal async Task<IEnumerable<string>> FetchPlayerData()
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
