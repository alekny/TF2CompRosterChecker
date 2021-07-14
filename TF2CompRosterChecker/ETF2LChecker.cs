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

using Newtonsoft.Json;
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
    class ETF2LChecker
    {
        private string[] steamIDs;
        private List<Player> noprofile = new List<Player>();
        public const string baseApiUrl = "https://api.etf2l.org/player/";
        public const string baseUrl = "https://etf2l.org/forum/user/";
        public const string baseTeamUrl = "https://etf2l.org/teams/";
        public const int HL = 0;
        public const int Sixes = 1;

        public ETF2LChecker(string statusOutput)
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
                if(index > SteamIDTools.RATECTRL)
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

        /*
         * A little bit lazy to invoke UI updates from this method, but whatevr ;>
         * TODO: Make this a little bit more elegant in the future.
         */
        [STAThread]
        public List<Player> parseJSON(int leagueformat, ProgressBar progressBar, Button button)
        {
            List<Player> playerlist = new List<Player>();
            var unique_ids = new HashSet<string>(this.steamIDs);
            int percentagefrac = 0;
            if (this.steamIDs.Length != 0)
            {
                percentagefrac = (int)(100 + unique_ids.Count) / unique_ids.Count;
            }
            //Only allow to check up to 50 SteamIDs per request.
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
                        string leagueid = "";
                        string steamid = SteamIDTools.steamID64ToSteamID(id);
                        string steamid3 = SteamIDTools.steamID64ToSteamID3(id);
                        string dl = "";
                        bool hasBans = false;
                        List<Ban> bans = new List<Ban>();
                        using (TimeoutWebClient wc = new TimeoutWebClient(8000))
                        {
                            wc.Encoding = Encoding.UTF8;
                            try
                            {
                                dl = wc.DownloadString(baseApiUrl + id + ".json");
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
                                                              "!![No ETF2L Profile]", 
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
                            name = (string)doc2["player"]["name"];
                            leagueid = (string)doc2["player"]["id"];
                            JArray teams = (JArray)doc2["player"]["teams"];
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

                            //Not that nice, there has to be a more elegant way?
                            try
                            {
                                JToken hit = teams.SelectToken(@"$.[?(@.type == '" + teamtype + "')]");
                                team = (string)hit["name"];
                                teamid = (string)hit["id"];

                                JObject comps = (JObject)hit["competitions"];
                                counter = -1;
                                foreach (var comp in comps)
                                {
                                    currentComp = Int32.Parse(comp.Key);
                                    currentDiv = (string)comp.Value["division"]["name"];
                                    if (counter < currentComp && currentDiv != null)
                                    {
                                        counter = currentComp;
                                        div = currentDiv;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                if (ex is NullReferenceException || ex is InvalidCastException)
                                {
                                    div = "";
                                }
                            }

                            try
                            {
                                JArray foundbans = doc2["player"]["bans"];
                                if (foundbans != null)
                                {
                                    hasBans = true;
                                    //Fetch specifics about found ban
                                    foreach (JToken ban in foundbans)
                                    {
                                        bans.Add(new Ban(ban["start"].ToString(), ban["end"].ToString(), ban["reason"].ToString()));
                                    }
                                }
                                else
                                {
                                    bans = null;
                                }
                            }
                            catch (NullReferenceException ne)
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
                                                      leagueid, 
                                                      hasBans, 
                                                      bans
                                                      ));
                        }
                    }

                    );
            return playerlist;
        }

        /*
         * Debug stuff.
         */
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
