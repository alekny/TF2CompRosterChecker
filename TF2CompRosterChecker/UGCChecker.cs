using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace TF2CompRosterChecker
{
    class UGCChecker
    {
        private string[] steamIDs;
        private List<Player> noprofile = new List<Player>();
        
        public static string baseUrl = "https://www.ugcleague.com/players_page.cfm?player_id=";
        public static string baseTeamUrl = "https://www.ugcleague.com/team_page.cfm?clan_id=";
        public static int HL = 0;
        public static int Sixes = 1;
        public static int FourVeeFour = 2;

        public UGCChecker(string statusOutput)
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
         * A little bit dirty, but UGC doesn't provide a proper API to work with.
         */
        [STAThread]
        public List<Player> parseUGCPlayerPage(int leagueformat, ProgressBar progressBar, Button button)
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
                    string webcontent = "";
                    string setDiv = "";
                    string setTeam = "";
                    string teamid = "";
                    using (WebClient wc = new WebClient())
                    {
                        try
                        {
                            webcontent = wc.DownloadString(string.Concat(baseUrl, id)).Replace("\n", string.Empty);
                            //Console.WriteLine(string.Concat(baseApiUrl, SteamIDTools.steamID3ToSteamID64(this.steamIDs[i])).Replace("\n", string.Empty));
                            //Console.WriteLine(webcontent);
                        }
                        catch (System.Net.WebException e)
                        {
                            playerlist.Add(new Player(id, "!![No UGC Profile]", "", "", id, "", false));
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

                        if (webcontent.Contains("No UGC TF2 League History"))
                        {
                            playerlist.Add(new Player(id, "!![No UGC Profile]", "", "", id, "", false));
                            return;
                        }
                        else
                        {
                            bool hasTeam = false;
                            bool hasBans = false;
                            var doc = new HtmlDocument();
                            string[] kek = this.steamIDs;
                            try
                            {
                                doc.LoadHtml(@webcontent);
                            }
                            catch (WebException w)
                            {
                                return;
                            }
                            //Console.WriteLine(id);
                            var h2 = doc.DocumentNode.Descendants("h2").FirstOrDefault();
                            string name = h2.Descendants("b").First().InnerHtml;
                            foreach (var node in doc.DocumentNode.Descendants("p"))
                            {
                                var div = node.Descendants("small").FirstOrDefault();
                                if (div != null)
                                {
                                    if (leagueformat == UGCChecker.HL)
                                    {
                                        if (div.InnerHtml.Contains("Highlander"))
                                        {
                                            string[] helper = div.InnerHtml.Split(new[] { "<br>" }, StringSplitOptions.None);
                                            setDiv = helper[1].Substring(17, helper[1].Length - 17);
                                        }
                                        else
                                        {
                                            continue;
                                        }
                                    }
                                    else if (leagueformat == UGCChecker.Sixes)
                                    {
                                        if (div.InnerHtml.Contains("6vs6"))
                                        {
                                            string[] helper = div.InnerHtml.Split(new[] { "<br>" }, StringSplitOptions.None);
                                            setDiv = helper[1].Substring(11, helper[1].Length - 11);
                                        }
                                        else
                                        {
                                            continue;
                                        }
                                    }
                                    else if (leagueformat == UGCChecker.FourVeeFour)
                                    {
                                        if (div.InnerHtml.Contains("4vs4"))
                                        {
                                            string[] helper = div.InnerHtml.Split(new[] { "<br>" }, StringSplitOptions.None);
                                            setDiv = helper[1].Substring(11, helper[1].Length - 11);
                                        }
                                        else
                                        {
                                            continue;
                                        }
                                    }

                                }
                                else
                                {
                                    continue;
                                }

                                var next = node.Descendants("span").FirstOrDefault();
                                if (next != null)
                                {
                                    var team = next.Descendants("b").FirstOrDefault();

                                    if (team != null && div != null)
                                    {
                                        setTeam = team.InnerHtml;
                                        teamid = node.Descendants("a").First().GetAttributeValue("href", "").Split('=')[1];
                                        hasTeam = true;

                                        break;
                                    }
                                }
                            }

                            var divs = doc.DocumentNode.Descendants("div");
                            foreach (HtmlNode node in divs)
                            {
                                var classes = node.GetClasses().FirstOrDefault();
                                if (classes != null)
                                {
                                    if (classes.Equals("col-md-10"))
                                    {
                                        var spans = node.Descendants("span").FirstOrDefault().GetClasses().FirstOrDefault();
                                        if (spans.Equals("text-danger"))
                                        {
                                            hasBans = true;
                                            break;
                                        }

                                    }
                                }
                            }

                            if (!hasTeam)
                            {
                                if (leagueformat == UGCChecker.HL)
                                {
                                    playerlist.Add(new Player(id, "![No UGC HL Team]", "", "", id, id, hasBans));
                                }
                                else if (leagueformat == UGCChecker.Sixes)
                                {
                                    playerlist.Add(new Player(id, "![No UGC 6v6 Team]", "", "", id, id, hasBans));
                                }
                                else if (leagueformat == UGCChecker.FourVeeFour)
                                {
                                    playerlist.Add(new Player(id, "![No UGC 4v4 Team]", "", "", id, id, hasBans));
                                }
                            }
                            else
                            {
                                playerlist.Add(new Player(name, setTeam, teamid, setDiv, id, id, hasBans));
                            }
                        }
                    }

                });
            return playerlist;
        }

        public string[] SteamIDS { get { return this.steamIDs; } }
    }
}
