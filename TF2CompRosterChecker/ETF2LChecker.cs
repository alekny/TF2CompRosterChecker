using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TF2CompRosterChecker
{
    internal sealed class ETF2LChecker : Checker
    {

        public ETF2LChecker(string statusOutput) : base(statusOutput)
        {
            BaseApiUrl = "https://api.etf2l.org/player/";
            BaseUrl = "https://etf2l.org/forum/user/";
            BaseTeamUrl = "https://etf2l.org/teams/";
        }

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
            //Only allow to check up to 50 SteamIDs per request.
            _ = Parallel.ForEach(unique_ids, new ParallelOptions { MaxDegreeOfParallelism = Checker.maxParallelThreads },
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
                        string steamid = SteamIDTools.SteamID64ToSteamID(id);
                        string steamid3 = SteamIDTools.SteamID64ToSteamID3(id);
                        string dl = "";
                        bool hasBans = false;
                        List<Ban> bans = new List<Ban>();


                        using (TimeoutWebClient wc = new TimeoutWebClient(8000))
                        {
                            wc.Encoding = Encoding.UTF8;
                            try
                            {
                                dl = wc.DownloadString(BaseApiUrl + id + ".json");
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

                            //Create a dynamic object for ease of use.
                            dynamic doc2 = JObject.Parse(dl);
                            name = (string)doc2["player"]["name"];
                            leagueid = (string)doc2["player"]["id"];
                            JArray teams = (JArray)doc2["player"]["teams"];
                            string teamtype = "";

                            if (leagueformat == LeagueFormat.HL)
                            {
                                teamtype = "Highlander";
                                team = "![No ETF2L HL Team]";
                            }
                            else if (leagueformat == LeagueFormat.Sixes)
                            {
                                teamtype = "6on6";
                                team = "![No ETF2L 6v6 Team]";
                            }

                            try
                            {
                                JToken hit = teams.SelectToken(@"$.[?(@.type == '" + teamtype + "')]");
                                team = (string)hit["name"];
                                teamid = (string)hit["id"];

                                JObject comps = (JObject)hit["competitions"];
                                counter = -1;
                                foreach (KeyValuePair<string, JToken> comp in comps)
                                {
                                    currentComp = int.Parse(comp.Key);
                                    currentDiv = (string)comp.Value["division"]["name"];
                                    if (counter < currentComp && currentDiv != null)
                                    {
                                        counter = currentComp;
                                        div = currentDiv;
                                    }
                                }


                            }
                            //If any Exception is caught here, there is no team data and no div
                            catch (Exception)
                            {
                                div = "";
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
                            catch (NullReferenceException)
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
    }
}
