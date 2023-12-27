
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace TF2CompRosterChecker
{
    public sealed class UGCChecker : Checker
    {
        public UGCChecker(string statusOutput) : base(statusOutput)
        {
            BaseUrl = "https://www.ugcleague.com/players_page.cfm?player_id=";
            BaseTeamUrl = "https://www.ugcleague.com/team_page.cfm?clan_id=";
        }

        /*
         * A little bit dirty to get our data from a html page, but UGC 
         * doesn't provide a proper API (xml or json) to work with.
         */
        [STAThread]
        public override List<Player> ParseData(LeagueFormat leagueformat, IProgress<int> progress)
        {
            List<Player> playerlist = new List<Player>();
            HashSet<string> unique_ids = new HashSet<string>(SteamIDs);
            int percentagefrac = 0;
            if (SteamIDs.Count != 0)
            {
                percentagefrac = (100 + unique_ids.Count) / unique_ids.Count;
            }
            _ = Parallel.ForEach(unique_ids, new ParallelOptions { MaxDegreeOfParallelism = Checker.maxParallelThreads },
                id =>
                {
                    string webcontent = "";
                    string setDiv = "";
                    string setTeam = "";
                    string teamid = "";
                    string steamid = SteamIDTools.SteamID64ToSteamID(id);
                    string steamid3 = SteamIDTools.SteamID64ToSteamID3(id);

                    using (CustomWebClient wc = new CustomWebClient(8000))
                    {
                        try
                        {
                            wc.Encoding = Encoding.UTF8;
                            webcontent = wc.DownloadString(string.Concat(BaseUrl, id)).Replace("\n", string.Empty);
                            //Console.WriteLine(string.Concat(baseApiUrl, SteamIDTools.steamID3ToSteamID64(this.steamIDs[i])).Replace("\n", string.Empty));
                            //Console.WriteLine(webcontent);
                        }
                        catch (WebException e)
                        {
                            if (e.Status == WebExceptionStatus.Timeout || e.Status == WebExceptionStatus.ConnectFailure)
                            {
                                playerlist.Add(new Player(
                                                          id,
                                                          "!![Connect Failure]",
                                                          "",
                                                          "",
                                                          id,
                                                          steamid,
                                                          steamid3,
                                                          "",
                                                          false,
                                                          null
                                                          ));
                            }
                            else
                            {
                                playerlist.Add(new Player(
                                                          id, 
                                                          "!![No UGC Profile]",
                                                          "",
                                                          "",
                                                          id,
                                                          steamid,
                                                          steamid3,
                                                          "",
                                                          false,
                                                          null
                                                          ));
                            }
                            if (progress != null)
                            {
                                progress.Report(percentagefrac);
                            }
                            return;
                        }
                        if (progress != null)
                        {
                            progress.Report(percentagefrac);
                        }
                     
                        if (webcontent.Contains("No UGC TF2 League History"))
                        {
                            playerlist.Add(new Player(
                                                      id,
                                                      "!![No UGC Profile]",
                                                      "",
                                                      "",
                                                      id,
                                                      steamid,
                                                      steamid3,
                                                      "",
                                                      false,
                                                      null
                                                      ));
                            return;
                        }
                        else
                        {
                            bool hasTeam = false;
                            bool hasBans = false;
                            HtmlDocument doc = new HtmlDocument();
                            try
                            {
                                doc.LoadHtml(@webcontent);
                            }
                            catch (WebException)
                            {
                                return;
                            }
                            //Console.WriteLine(id);

                            string name = "";
                            try
                            {
                                IEnumerable<HtmlNode> h3 = doc.DocumentNode.Descendants("h3");
                                foreach (HtmlNode desc in h3)
                                {
                                    if ("b".Equals(desc.FirstChild.Name))
                                    {
                                        name = desc.FirstChild.InnerHtml;
                                    }
                                }
                                foreach (HtmlNode node in doc.DocumentNode.Descendants("p"))
                                {
                                    HtmlNode div = node.Descendants("small").FirstOrDefault();
                                    if (div != null)
                                    {
                                        if (leagueformat == LeagueFormat.HL)
                                        {
                                            if (div.InnerHtml.Contains("Highlander"))
                                            {
                                                string[] helper = div.InnerHtml.Split(new[] { "<br>" }, StringSplitOptions.None);
                                                setDiv = helper[1].Substring(17, helper[1].Length - 17).Replace("New Teams", "NT");
                                            }
                                            else
                                            {
                                                continue;
                                            }
                                        }
                                        else if (leagueformat == LeagueFormat.Sixes)
                                        {
                                            if (div.InnerHtml.Contains("6vs6"))
                                            {
                                                string[] helper = div.InnerHtml.Split(new[] { "<br>" }, StringSplitOptions.None);
                                                setDiv = helper[1].Substring(11, helper[1].Length - 11).Replace("New Teams", "NT");
                                            }
                                            else
                                            {
                                                continue;
                                            }
                                        }
                                        else if (leagueformat == LeagueFormat.FourVeeFour)
                                        {
                                            if (div.InnerHtml.Contains("4vs4"))
                                            {
                                                string[] helper = div.InnerHtml.Split(new[] { "<br>" }, StringSplitOptions.None);
                                                setDiv = helper[1].Substring(11, helper[1].Length - 11).Replace("New Teams", "NT");
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

                                    HtmlNode next = node.Descendants("span").FirstOrDefault();
                                    if (next != null)
                                    {
                                        HtmlNode team = next.Descendants("b").FirstOrDefault();

                                        if (team != null && div != null)
                                        {
                                            setTeam = team.InnerHtml;
                                            teamid = node.Descendants("a").First().GetAttributeValue("href", "").Split('=')[1];
                                            hasTeam = true;

                                            break;
                                        }
                                    }
                                }
                            }
                             catch (NullReferenceException)
                            {
                                //HTML tag wasn't found, probably due to a design change...
                            }
                            
                            try
                            {
                                IEnumerable<HtmlNode> divs = doc.DocumentNode.Descendants("div");
                                foreach (HtmlNode node in divs)
                                {
                                    string classes = node.GetClasses().FirstOrDefault();
                                    if (classes != null)
                                    {
                                        if (classes.Equals("col-md-10"))
                                        {
                                            string spans = node.Descendants("span").FirstOrDefault().GetClasses().FirstOrDefault();
                                            if (spans.Equals("text-danger"))
                                            {
                                                hasBans = true;
                                                break;
                                            }

                                        }
                                    }
                                }
                            }

                            catch (NullReferenceException)
                            {
                                //HTML tag wasn't found, probably due to a design change...
                            }

                            if (!hasTeam)
                            {
                                if (leagueformat == LeagueFormat.HL)
                                {
                                    playerlist.Add(new Player(
                                                              name,
                                                              "![No UGC HL Team]",
                                                              "",
                                                              "",
                                                              id,
                                                              steamid,
                                                              steamid3,
                                                              id,
                                                              hasBans,
                                                              null
                                                              ));
                                }
                                else if (leagueformat == LeagueFormat.Sixes)
                                {
                                    playerlist.Add(new Player(
                                                              name,
                                                              "![No UGC 6v6 Team]",
                                                              "",
                                                              "",
                                                              id,
                                                              steamid,
                                                              steamid3,
                                                              id,
                                                              hasBans,
                                                              null
                                                              ));
                                }
                                else if (leagueformat == LeagueFormat.FourVeeFour)
                                {
                                    playerlist.Add(new Player(
                                                              name,
                                                              "![No UGC 4v4 Team]",
                                                              "",
                                                              "",
                                                              id,
                                                              steamid,
                                                              steamid3,
                                                              id,
                                                              hasBans,
                                                              null
                                                              ));
                                }
                            }
                            else
                            {
                                playerlist.Add(new Player(
                                                          name,
                                                          setTeam,
                                                          teamid,
                                                          setDiv,
                                                          id,
                                                          steamid,
                                                          steamid3,
                                                          id,
                                                          hasBans,
                                                          null
                                                          ));
                            }
                        }
                    }

                });
            return playerlist;
        }
    }
}
