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
    class RGLChecker
    {
        private string[] steamIDs;
        private List<Player> noprofile = new List<Player>();
        public const string baseApiUrl = "https://rgl.payload.tf/api/v1/profiles/";
        public const string baseUrl = "https://rgl.gg/Public/PlayerProfile.aspx?p=";

        //For now just display the Player page (the payload api response has no direct link to any player's team id)
        public const string baseTeamUrl = "https://rgl.gg/Public/PlayerProfile.aspx?p=";
        public const int HL = 0;
        public const int PL = 1;
        public const int TradSixes = 2;
        public const int NRSixes = 3;


        public RGLChecker(string statusOutput)
        {
            int index = 0;
            MatchCollection matchesSteamID = Regex.Matches(statusOutput, SteamIDTools.steamIDregex);
            MatchCollection matchesSteamID3 = Regex.Matches(statusOutput, SteamIDTools.steamID3regex);
            MatchCollection matchesProfileUrl = Regex.Matches(statusOutput, SteamIDTools.profileUrlregex);
            MatchCollection matchesProfileCustomUrl = Regex.Matches(statusOutput, SteamIDTools.profileCustomUrlregex);
            string[] foundSteamIDs = new string[matchesSteamID.Count + matchesSteamID3.Count + matchesProfileUrl.Count + matchesProfileCustomUrl.Count];
            foreach (Match match in matchesSteamID3)
            {
                //Limit Max Results to 50 to not flood the apis.
                if (index > SteamIDTools.RATECTRL)
                {
                    break;
                }
                foundSteamIDs[index] = SteamIDTools.steamID3ToSteamID64(match.ToString());
                index++;
            }
            foreach (Match match in matchesSteamID)
            {
                if (index > SteamIDTools.RATECTRL)
                {
                    break;
                }
                foundSteamIDs[index] = SteamIDTools.steamIDToSteamID64(match.ToString());
                index++;
            }
            foreach (Match match in matchesProfileUrl)
            {
                if (index > SteamIDTools.RATECTRL)
                {
                    break;
                }
                foundSteamIDs[index] = match.Groups[1].ToString();
                index++;
            }
            foreach (Match match in matchesProfileCustomUrl)
            {
                if (index > SteamIDTools.RATECTRL)
                {
                    break;
                }
                using (TimeoutWebClient wc = new TimeoutWebClient(8 * 1000))
                {
                    wc.Encoding = Encoding.UTF8;
                    try
                    {
                        string dl = wc.DownloadString(match.ToString() + "/?xml=1");
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(dl);
                        XmlNodeList results = doc.GetElementsByTagName("steamID64");

                        //Check if the steam profile even exists.
                        if (results.Count == 1)
                        {
                            foundSteamIDs[index] = results.Item(0).InnerText;
                        }
                    }
                    catch (System.Net.WebException e)
                    {
                        // do nothing lul
                    }
                }
                
                index++;
            }
            this.steamIDs = foundSteamIDs;
        }


        [STAThread]
        public List<Player> ParseJSON(int leagueformat, ProgressBar progressBar, Button button)
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
                        //Initialize variables for each Player instance.
                        int currentComp = -1;
                        int counter = -1;
                        string currentDiv = "";
                        string name = "";
                        string team = "";
                        string teamid = "";
                        string div = "";
                        string dl = "";
                        bool hasBans = false;
                        string steamid = SteamIDTools.steamID64ToSteamID(id);
                        string steamid3 = SteamIDTools.steamID64ToSteamID3(id);

                        //Using a modified webclient, because the payload.tf api is quite slow (mostly)
                        //This will introduce another problem (players that are indeed registered at rgl
                        //will be shown as unregistered, if the timout is reached), but at least the
                        //program wont hang for 100...
                        using (TimeoutWebClient wc = new TimeoutWebClient(8 * 1000))
                        {
                            wc.Encoding = Encoding.UTF8;
                            try
                            {
                                dl = wc.DownloadString(baseApiUrl + id);
                            }
                            catch (System.Net.WebException e)
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

                            

                            //Create a dynamic object for ease of use.
                            dynamic doc2 = JObject.Parse(dl);

                            if (doc2["statusCode"] == null && doc2["data"]["statusCode"] == null)
                            {
                                name = (string)doc2["data"]["name"];
                                JArray teams = (JArray)doc2["data"]["experience"];
                                string teamtype = "";

                                if (leagueformat == RGLChecker.HL)
                                {
                                    teamtype = "na highlander";
                                    team = "![No RGL HL Team]";
                                }
                                else if (leagueformat == RGLChecker.PL)
                                {
                                    teamtype = "na prolander";
                                    team = "![No RGL PL Team]";
                                }
                                else if (leagueformat == RGLChecker.TradSixes)
                                {
                                    teamtype = "na traditional sixes";
                                    team = "![No RGL trad. 6v6 Team]";
                                }
                                else if (leagueformat == RGLChecker.NRSixes)
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
                                catch (Exception ne)
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