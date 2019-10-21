using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Xml;

namespace TF2CompRosterChecker
{
    class ETF2LChecker
    {
        private string[] steamIDs;
        private List<Player> noprofile = new List<Player>();
        public static string baseApiUrl = "http://api.etf2l.org/player/";
        public static string baseUrl = "http://etf2l.org/forum/user/";
        public static string baseTeamUrl = "http://etf2l.org/teams/";
        public static int HL = 0;
        public static int Sixes = 1;

        public ETF2LChecker(string statusOutput)
        {
            int index = 0;
            MatchCollection matchesSteamID3 = Regex.Matches(statusOutput, SteamIDTools.steamID3regex);
            MatchCollection matchesProfileUrl = Regex.Matches(statusOutput, SteamIDTools.profileUrlregex);
            //MatchCollection matchesProfileCustomUrl = Regex.Matches(statusOutput, SteamIDTools.profileCustomUrlregex);
            string[] foundSteamIDs = new string[matchesSteamID3.Count + matchesProfileUrl.Count /*+ matchesProfileCustomUrl.Count*/];
            foreach (Match match in matchesSteamID3)
            {
                foundSteamIDs[index] = SteamIDTools.steamID3ToSteamID64(match.ToString());
                index++;
            }
            foreach (Match match in matchesProfileUrl)
            {
                foundSteamIDs[index] = match.Groups[1].ToString();
                index++;
            }
            //TODO: Make this crap work.
            /*Parallel.ForEach(matchesProfileCustomUrl.OfType<Match>(),
                match =>
                {
                    foundSteamIDs[index] = SteamIDTools.getSteamID64FromCustomUrl(match.ToString());
                    index++;
                });*/

            this.steamIDs = foundSteamIDs;
        }

        /*
         * A little bit lazy to invoke UI updates from this method, but whatevr ;>
         * TODO: Make this a little bit more elegant in the future.
         */
        [STAThread]
        public List<Player> parseXML(int leagueformat, ProgressBar progressBar, Button button)
        {
            List<Player> playerlist = new List<Player>();
            var unique_ids = new HashSet<string>(this.steamIDs);
            int percentagefrac = 0;
            if (this.steamIDs.Length != 0)
            {
                percentagefrac = (int)(100 + unique_ids.Count) / unique_ids.Count;
            }
            Parallel.ForEach(unique_ids, 
                    id =>
                    {
                        int currentComp = -1;
                        int counter = -1;
                        string currentDiv = "";
                        string name = "";
                        string team = "";
                        string teamid = "";
                        string div = "";
                        string profileid = "";
                        string dl = "";
                        bool hasBans = false;
                        using (WebClient wc = new WebClient())
                        {
                            wc.Encoding = Encoding.UTF8;
                            try
                            {
                                dl = wc.DownloadString(baseApiUrl + id + ".xml");
                            }
                            catch (System.Net.WebException e)
                            {
                                playerlist.Add(new Player(id, "!![No ETF2L Profile]", "", "", id, "", false));
                                if (progressBar != null)
                                {
                                    progressBar.Dispatcher.Invoke(() => progressBar.Value += percentagefrac, DispatcherPriority.Background);
                                }
                                if (button != null)
                                {
                                    button.Dispatcher.Invoke(() => button.Content = "Checking: " + progressBar.Value + "%", DispatcherPriority.Background);
                                }
                                return;
                            }

                            if (progressBar != null)
                            {
                                progressBar.Dispatcher.Invoke(() => progressBar.Value += percentagefrac, DispatcherPriority.Background);
                            }
                            if (button != null)
                            {
                                button.Dispatcher.Invoke(() => button.Content = "Checking: " + progressBar.Value + "%", DispatcherPriority.Background);
                            }

                            XmlDocument doc = new XmlDocument();
                            doc.LoadXml(dl);
                            XmlNodeList nodes = doc.GetElementsByTagName("player");
                            name = nodes.Item(0).Attributes["name"].Value;
                            profileid = nodes.Item(0).Attributes["id"].Value;
                            XmlNodeList teams = doc.GetElementsByTagName("teams");
                            string teamtype = "";
                            if (leagueformat == ETF2LChecker.HL)
                            {
                                teamtype = "Highlander";
                                team = "![No ETF2L HL Team]";
                            }
                            else if (leagueformat == ETF2LChecker.Sixes)
                            {
                                teamtype = "6on6";
                                team = "![No ETF2L 6v6 Team]";
                            }
                            foreach (XmlNode t in teams)
                            {
                                if (t.Attributes["type"].Value.Equals(teamtype))
                                {
                                    team = t.Attributes["name"].Value;
                                    teamid = t.Attributes["id"].Value;
                                    break;
                                }
                            }
                            XmlNodeList competitions = doc.GetElementsByTagName("competitions");
                            counter = -1;
                            foreach (XmlNode c in competitions)
                            {
                                string league = "";
                                if (leagueformat == ETF2LChecker.HL)
                                {
                                    league = "Highlander Season";
                                }
                                else if (leagueformat == ETF2LChecker.Sixes)
                                {
                                    league = "6v6 Season";
                                }

                                if (c.Attributes["category"].Value.Equals(league))
                                {
                                    counter = Int32.Parse(c.Attributes["name"].Value);
                                    currentDiv = c.ChildNodes.Item(0).Attributes["name"].Value;
                                    if (counter > currentComp && !currentDiv.Equals("") && currentDiv != null)
                                    {
                                        div = currentDiv;
                                        currentComp = counter;
                                    }
                                }
                            }

                            if (doc.GetElementsByTagName("bans").Item(0) != null)
                            {
                                hasBans = true;
                            }

                            playerlist.Add(new Player(name, team, teamid, div, id, profileid, hasBans));
                        }
                    }

                    );
            return playerlist;
        }

        public void printIDs()
        {
            for (int i = 0; i < this.steamIDs.Length; i++)
            {
                Console.WriteLine(this.steamIDs[i]);
            }
        }

        public string[] SteamIDS { get { return this.steamIDs; } }
    }
}
