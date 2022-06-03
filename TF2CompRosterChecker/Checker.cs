
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace TF2CompRosterChecker
{
    public abstract class Checker
    {
        public enum LeagueFormat
        {
            HL,
            Sixes,
            PL,
            NRSixes,
            FourVeeFour
        }

        public const int maxParallelThreads = 5;
        public const int RATECTRL = 100;
        public abstract List<Player> ParseData(LeagueFormat leagueformat, IProgress<int> progress);
        public List<string> SteamIDs { get; set; }
        public string BaseApiUrl { get; set; }
        public string BaseUrl { get; set; }
        public string BaseTeamUrl { get; set; }

        public Checker(string statusOutput)
        {
            int index = 0;
            MatchCollection matchesSteamID = Regex.Matches(statusOutput, SteamIDTools.steamIDregex);
            MatchCollection matchesSteamID3 = Regex.Matches(statusOutput, SteamIDTools.steamID3regex);
            MatchCollection matchesProfileUrl = Regex.Matches(statusOutput, SteamIDTools.profileUrlregex);
            MatchCollection matchesProfileCustomUrl = Regex.Matches(statusOutput, SteamIDTools.profileCustomUrlregex);
            List<string> foundSteamIDs = new List<string>();
            //We cannot use fixed arrays anymore, since a valid custom profile url doesnt necessarily lead
            //to a valid steam id. So List it is.
            //string[] foundSteamIDs = new string[matchesSteamID.Count + matchesSteamID3.Count + matchesProfileUrl.Count + matchesProfileCustomUrl.Count];
            foreach (Match match in matchesSteamID3)
            {
                //Limit Max Results to 50 to not flood the apis.
                if (index > RATECTRL)
                {
                    break;
                }
                foundSteamIDs.Add(SteamIDTools.SteamID3ToSteamID64(match.ToString()));
                index++;
            }
            foreach (Match match in matchesSteamID)
            {
                if (index > RATECTRL)
                {
                    break;
                }
                foundSteamIDs.Add(SteamIDTools.SteamIDToSteamID64(match.ToString()));
                index++;
            }
            foreach (Match match in matchesProfileUrl)
            {
                if (index > RATECTRL)
                {
                    break;
                }
                foundSteamIDs.Add(match.Groups[1].ToString());
                index++;
            }
            foreach (Match match in matchesProfileCustomUrl)
            {
                if (index > RATECTRL)
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
                            foundSteamIDs.Add(results.Item(0).InnerText);
                        }
                        index++;
                    }
                    catch (System.Net.WebException)
                    {
                        // do nothing lul
                    }
                }
            }

            SteamIDs = foundSteamIDs;
        }

        /*
         * Debug stuff.
         */
        public void PrintIDs()
        {
            foreach (string steamID in SteamIDs)
            {
                Console.WriteLine(steamID);
            }
        }

    }
}
