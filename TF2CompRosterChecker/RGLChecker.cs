/**
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

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Xml;

namespace TF2CompRosterChecker
{
    internal class RGLChecker : Checker
    {
        public RGLChecker(string statusOutput) : base(statusOutput)
        {
            BaseApiUrl = "https://rgl.payload.tf/api/v1/profiles/";
            BaseUrl = "https://rgl.gg/Public/PlayerProfile.aspx?p=";

            //For now just display the Player page (the payload api response has no direct link to any player's team id)
            BaseTeamUrl = "https://rgl.gg/Public/PlayerProfile.aspx?p=";

        }

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
                        //Initialize variables for each Player instance.
                        string name = "";
                        string team = "";
                        string teamid = "";
                        string div = "";
                        string dl = "";
                        bool hasBans = false;
                        string steamid = SteamIDTools.SteamID64ToSteamID(id);
                        string steamid3 = SteamIDTools.SteamID64ToSteamID3(id);

                        //Using a modified webclient, because the payload.tf api is quite slow (mostly)
                        //This will introduce another problem (players that are indeed registered at rgl
                        //will be shown as unregistered, if the timout is reached), but at least the
                        //program wont hang for 100...
                        using (TimeoutWebClient wc = new TimeoutWebClient(8 * 1000))
                        {
                            wc.Encoding = Encoding.UTF8;
                            try
                            {
                                dl = wc.DownloadString(BaseApiUrl + id);
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
                                                              "!![No RGL Profile]",
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



                            //Create a dynamic object for ease of use.
                            dynamic doc2 = JObject.Parse(dl);

                            if (doc2["statusCode"] == null && doc2["data"]["statusCode"] == null)
                            {
                                name = (string)doc2["data"]["name"];
                                JArray teams = (JArray)doc2["data"]["experience"];
                                string teamtype = "";

                                if (leagueformat == Checker.HL)
                                {
                                    teamtype = "na highlander";
                                    team = "![No RGL HL Team]";
                                }
                                else if (leagueformat == Checker.PL)
                                {
                                    teamtype = "na prolander";
                                    team = "![No RGL PL Team]";
                                }
                                else if (leagueformat == Checker.Sixes)
                                {
                                    teamtype = "na traditional sixes";
                                    team = "![No RGL trad. 6v6 Team]";
                                }
                                else if (leagueformat == Checker.NRSixes)
                                {
                                    teamtype = "na no restriction sixes";
                                    team = "![No RGL nr 6v6 Team]";
                                }


                                //Not that nice, there has to be a more elegant way?
                                try
                                {
                                    IEnumerable<JToken> hit = teams.SelectTokens(@"$.[?(@.format == '" + teamtype + "')]");

                                    foreach (JToken entry in hit)
                                    {
                                        if ((bool)entry["isCurrentTeam"])
                                        {
                                            //Workaround for now
                                            teamid = id;
                                            team = (string)entry["team"];
                                            div = (string)entry["div"];
                                        }
                                    }

                                }
                                catch (Exception ex)
                                {
                                    if (ex is NullReferenceException || ex is InvalidCastException)
                                    {
                                        div = "[inactive]";
                                    }
                                }

                                try
                                {
                                    if ((bool)doc2["data"]["status"]["banned"] || (bool)doc2["data"]["status"]["probation"])
                                    {
                                        hasBans = true;
                                    }
                                }
                                catch (Exception)
                                {
                                    //Do nothing in this case...
                                }


                                playerlist.Add(new Player(
                                                          name,
                                                          team,
                                                          teamid,
                                                          div,
                                                          id,
                                                          steamid,
                                                          steamid3,
                                                          id,
                                                          hasBans,
                                                          null
                                                          ));
                            }
                            else
                            {
                                playerlist.Add(new Player(
                                                          id,
                                                          "!![No RGL Profile]",
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
                        }
                    }

                    );
            return playerlist;
        }

    }
}