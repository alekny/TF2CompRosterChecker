﻿/**
 * 
 *  Costura.Fody
 *  Copyright (c) Simon Cropp, Cameron MacFarland
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy 
 * of this software and associated documentation files (the "Software"), to deal 
 * in the Software without restriction, including without limitation the rights 
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
 * copies of the Software, and to permit persons to whom the Software is 
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in 
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
 * IN THE SOFTWARE.
 * 
 * -----------------------------------------------------------------------------
 * 
 *  Fody
 *  Copyright (c) Simon Cropp
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy 
 * of this software and associated documentation files (the "Software"), to deal 
 * in the Software without restriction, including without limitation the rights 
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
 * copies of the Software, and to permit persons to whom the Software is 
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in 
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
 * IN THE SOFTWARE.
 * 
 * -----------------------------------------------------------------------------
 * 
 * 
 *  Html Agility Pack (HAP)
 *  Copyright (c)ZZZ Projects, Simon Mourrier, Jeff Klawiter, Stephan Grell
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy 
 * of this software and associated documentation files (the "Software"), to deal 
 * in the Software without restriction, including without limitation the rights 
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
 * copies of the Software, and to permit persons to whom the Software is 
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in 
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
 * IN THE SOFTWARE.
 * 
 * -----------------------------------------------------------------------------
 * 
 *  Newtonsoft.Json
 *  Copyright (c) James Newton-King
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy 
 * of this software and associated documentation files (the "Software"), to deal 
 * in the Software without restriction, including without limitation the rights 
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
 * copies of the Software, and to permit persons to whom the Software is 
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in 
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
 * IN THE SOFTWARE.
 * 
 */

using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace TF2CompRosterChecker
{
    internal class UGCChecker : Checker
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
        public override List<Player> ParseData(int leagueformat, ProgressBar progressBar, Button button)
        {
            List<Player> playerlist = new List<Player>();
            HashSet<string> unique_ids = new HashSet<string>(SteamIDs);
            int percentagefrac = 0;
            if (SteamIDs.Count != 0)
            {
                percentagefrac = (100 + unique_ids.Count) / unique_ids.Count;
            }
            _ = Parallel.ForEach(unique_ids,
                id =>
                {
                    string webcontent = "";
                    string setDiv = "";
                    string setTeam = "";
                    string teamid = "";
                    string steamid = SteamIDTools.SteamID64ToSteamID(id);
                    string steamid3 = SteamIDTools.SteamID64ToSteamID3(id);
                    using (TimeoutWebClient wc = new TimeoutWebClient(8000))
                    {
                        try
                        {
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
                            
                            if (progressBar != null)
                            {
                                _ = progressBar.Dispatcher.Invoke(() => progressBar.Value += percentagefrac, DispatcherPriority.Background);
                            }
                            if (button != null)
                            {
                                _ = button.Dispatcher.Invoke(() => button.Content = "Checking: " + progressBar.Value + "%", DispatcherPriority.Background);
                            }
                            return;
                        }

                        if (progressBar != null)
                        {
                            _ = progressBar.Dispatcher.Invoke(() => progressBar.Value += percentagefrac, DispatcherPriority.Background);
                        }
                        if (button != null)
                        {
                            _ = button.Dispatcher.Invoke(() => button.Content = "Checking: " + progressBar.Value + "%", DispatcherPriority.Background);
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
                                        if (leagueformat == Checker.HL)
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
                                        else if (leagueformat == Checker.Sixes)
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
                                        else if (leagueformat == Checker.FourVeeFour)
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
                                if (leagueformat == Checker.HL)
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
                                else if (leagueformat == Checker.Sixes)
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
                                else if (leagueformat == Checker.FourVeeFour)
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
