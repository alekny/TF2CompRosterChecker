
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TF2CompRosterChecker
{
    internal sealed class RGLChecker : Checker
    {

        public RGLChecker(string statusOutput) : base(statusOutput)
        {
            BaseApiUrl = "https://rgl.gg/Public/PlayerProfile.aspx?p=";
            BaseUrl = "https://rgl.gg/Public/PlayerProfile.aspx?p=";
            BaseTeamUrl = "https://rgl.gg/Public/Team.aspx?t=";
        }

        public static long ToUnixTimestamp(DateTime target)
        {
            DateTime date = new DateTime(1970, 1, 1, 0, 0, 0, target.Kind);
            long unixTimestamp = Convert.ToInt64((target - date).TotalSeconds);

            return unixTimestamp;
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

            _ = Parallel.ForEach(unique_ids, new ParallelOptions { MaxDegreeOfParallelism = Checker.maxParallelThreads }, 
                    id =>
                    {
                        //Initialize variables for each Player instance.
                        string webcontent = "";
                        string name = "";
                        string team = "";
                        string teamid = "";
                        string div = "";
                        bool hasBans = false;
                        string steamid = SteamIDTools.SteamID64ToSteamID(id);
                        string steamid3 = SteamIDTools.SteamID64ToSteamID3(id);
                        List<Ban> bans = null;

                        

                        //Using a modified webclient, because the rgl website sometimes responds quite slowly.
                        //This will introduce another problem (players that are indeed registered at rgl
                        //will be shown as unregistered, if the timout is reached), but at least the
                        //program wont hang for 100 seconds...

                        using (TimeoutWebClient wc = new TimeoutWebClient(8 * 1000))
                        {
                            wc.Encoding = Encoding.UTF8;
                            try
                            {
                                webcontent = wc.DownloadString(string.Concat(BaseUrl, id)).Replace("\n", string.Empty);
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

                            if (webcontent.Contains("Player does not exist in RGL"))
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
                                return;
                            }
                            else
                            {
                                bool hasTeam = false;
                                HtmlDocument doc = new HtmlDocument();
                                try
                                {
                                    doc.LoadHtml(@webcontent);
                                }
                                catch (WebException)
                                {
                                    return;
                                }

                                try
                                {
                                    //This apparently has some problems with names containing "@" (cloudflare email protection).
                                    //Those will show "email protected" as current name, the rest should work properly tho.
                                    name = doc.GetElementbyId("ContentPlaceHolder1_ContentPlaceHolder1_ContentPlaceHolder1_lblPlayerName").InnerText;

                                }
                                catch (NullReferenceException)
                                {
                                    //HTML tag wasn't found, probably due to a frontend design change...
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
                                    return;
                                }

                                try
                                {
                                    if (!doc.GetElementbyId("ContentPlaceHolder1_ContentPlaceHolder1_ContentPlaceHolder1_lblPlayerBanHistory").InnerText.Equals("Player has no discipline history"))
                                    {
                                        hasBans = true;
                                        bans = new List<Ban>();
                                        HtmlNode bantitle = doc.DocumentNode.SelectSingleNode("//h3[@id=\"banhistory\"]");
                                        HtmlNode bantable = bantitle.NextSibling.NextSibling.ChildNodes["table"];
                                        string[] startdate, enddate;
                                        long unixstart, unixend = -1;
                                        string reason = "";
                                        foreach (HtmlNode row in bantable.SelectNodes("tr"))
                                        {
                                            //Skip the title row
                                            if (row.SelectSingleNode("(th|td)[1]").InnerText == "Active")
                                            {
                                                continue;
                                            }
                                            startdate = row.SelectSingleNode("(th|td)[2]").InnerText.Trim().Split('/');
                                            //Contains correct Date
                                            if (startdate.Length == 3)
                                            {
                                                unixstart = ToUnixTimestamp(new DateTime(int.Parse(startdate[2]), int.Parse(startdate[0]), int.Parse(startdate[1])));
                                                
                                            }
                                            else
                                            {
                                                unixstart = 0;
                                            }
                                            enddate = row.SelectSingleNode("(th|td)[3]").InnerText.Trim().Split('/');
                                            //If there is no data given, it means this column contains "Permanent"
                                            if (enddate.Length == 3)
                                            {
                                                unixend = ToUnixTimestamp(new DateTime(int.Parse(enddate[2]), int.Parse(enddate[0]), int.Parse(enddate[1])));
                                            }
                                            else
                                            {
                                                //This essientially means Permaban!
                                                unixend = 2147483647;
                                            }
                                            reason = row.SelectSingleNode("(th|td)[4]").InnerText.Trim();
                                            bans.Add(new Ban(unixstart.ToString(), unixend.ToString(), reason));
                                        }
                                    }
                                }
                                catch (Exception)
                                {
                                    //A bit lazy but for now it's fine (to prevent crashes if RGL changes html stuff)
                                }

                                try
                                {
                                    HtmlNode leagueNode = null;
                                    HtmlNode tablepointer = null;
                                    HtmlNode table = null;
                                    string leaguename = "";

                                    if (leagueformat == LeagueFormat.Sixes)
                                    {
                                        //Sixes
                                        leagueNode = doc.GetElementbyId("ContentPlaceHolder1_ContentPlaceHolder1_ContentPlaceHolder1_rptLeagues_lblLeagueName_0");
                                        leaguename = "Trad. Sixes";
                                    }
                                    else if (leagueformat == LeagueFormat.HL)
                                    {
                                        //Highlander
                                        leagueNode = doc.GetElementbyId("ContentPlaceHolder1_ContentPlaceHolder1_ContentPlaceHolder1_rptLeagues_lblLeagueName_1");
                                        leaguename = "Highlander";
                                    }
                                    else if (leagueformat == LeagueFormat.PL)
                                    {
                                        //Prolander
                                        leagueNode = doc.GetElementbyId("ContentPlaceHolder1_ContentPlaceHolder1_ContentPlaceHolder1_rptLeagues_lblLeagueName_2");
                                        leaguename = "Prolander";
                                    }

                                    if (leagueNode != null)
                                    {
                                        HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//h3");

                                        foreach (HtmlNode title in nodes)
                                        {
                                            if (title.InnerHtml.Contains(leaguename))
                                            {
                                                tablepointer = title;
                                                break;
                                            }
                                        }

                                        if (tablepointer != null)
                                        {
                                            //May God have mercy on my soul
                                            table = tablepointer.NextSibling.NextSibling.NextSibling.NextSibling.ChildNodes["table"];
                                            HtmlNode temptd = null;
                                            foreach (HtmlNode row in table.SelectNodes("tr"))
                                            {
                                                if (row.Attributes["style"]?.Value != null)
                                                {
                                                    temptd = row.SelectSingleNode("(th|td)[3]").SelectSingleNode("a");
                                                    //That Last bit: WTF? Why is it even necessary to do this??
                                                    team = temptd.InnerText.Trim();
                                                    //At first it might not make sense to split the id from the rest of
                                                    //the url, but this should prevent link injections.
                                                    teamid = temptd.Attributes["href"].Value.Remove(0, BaseTeamUrl.Length);
                                                    div = row.SelectSingleNode("(th|td)[2]").InnerText.Trim();
                                                    //Too long, remember when divs were all numbers?
                                                    if (div == "Admin Placement")
                                                    {
                                                        div = "Admin Plcmt.";
                                                    }
                                                    hasTeam = true;
                                                    break;
                                                }
                                            }
                                            if (hasTeam)
                                            {
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
                                                              bans
                                                              ));
                                            }
                                            else
                                            {
                                                playerlist.Add(new Player(
                                                              name,
                                                              "![No RGL " + leaguename + " Team]",
                                                              "",
                                                              "",
                                                              id,
                                                              steamid,
                                                              steamid3,
                                                              id,
                                                              hasBans,
                                                              bans
                                                              ));
                                            }
                                            return;
                                        }
                                        else
                                        {
                                            playerlist.Add(new Player(
                                                              name,
                                                              "![No RGL " + leaguename + " Team]",
                                                              "",
                                                              "",
                                                              id,
                                                              steamid,
                                                              steamid3,
                                                              id,
                                                              hasBans,
                                                              bans
                                                              ));
                                            return;
                                        }

                                    }
                                    {
                                        playerlist.Add(new Player(
                                                              name,
                                                              "![No RGL " + leaguename + " Team]",
                                                              "",
                                                              "",
                                                              id,
                                                              steamid,
                                                              steamid3,
                                                              id,
                                                              hasBans,
                                                              bans
                                                              ));
                                        return;
                                    }

                                }
                                catch (NullReferenceException)
                                {
                                    //HTML tag wasn't found, probably due to a frontend design change...
                                }
                            }

                        }
                    }

                    );
            return playerlist;
        }

    }
}