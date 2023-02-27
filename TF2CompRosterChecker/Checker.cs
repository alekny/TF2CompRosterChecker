
using Newtonsoft.Json.Linq;
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

        public delegate void Printer(string message);

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

            //TODO: Put all those in a List maybe.
            MatchCollection matchesSteamID = Regex.Matches(statusOutput, SteamIDTools.steamIDregex);
            MatchCollection matchesSteamID3 = Regex.Matches(statusOutput, SteamIDTools.steamID3regex);
            MatchCollection matchesProfileUrl = Regex.Matches(statusOutput, SteamIDTools.profileUrlregex);
            MatchCollection matchesProfileCustomUrl = Regex.Matches(statusOutput, SteamIDTools.profileCustomUrlregex);
            MatchCollection matchesEtf2lUrl = Regex.Matches(statusOutput, SteamIDTools.etf2lProfileUrl);
            MatchCollection matchesUgcUrl = Regex.Matches(statusOutput, SteamIDTools.ugcProfileUrl);
            MatchCollection matchesRglUrl = Regex.Matches(statusOutput, SteamIDTools.rglProfileUrl);
            MatchCollection matchesTf2centerUrl = Regex.Matches(statusOutput, SteamIDTools.tf2centerProfileUrl);
            List<string> foundSteamIDs = new List<string>();

            foreach (Match match in matchesSteamID3)
            {
                //Limit Max Results to RATECTRL to not flood the apis.
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
                            index++;
                        }
                    }
                    catch (System.Net.WebException)
                    {
                        // do nothing
                    }
                }
            }
            foreach (Match match in matchesEtf2lUrl)
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
                        //uglyyyyyy
                        string dl = wc.DownloadString("https://api.etf2l.org/player/" + match.Groups[1].ToString() + ".json");
                        dynamic doc = JObject.Parse(dl);
                        string steamid = (string)doc["player"]["steam"]["id64"];

                        //Check if the steam profile even exists.
                        if (steamid != null)
                        {
                            foundSteamIDs.Add(steamid);
                            index++;
                        }
                    }
                    catch (Exception)
                    {
                        // do nothing
                    }
                }
            }
            foreach (Match match in matchesUgcUrl)
            {
                if (index > RATECTRL)
                {
                    break;
                }
                foundSteamIDs.Add(match.Groups[1].ToString());
                index++;
            }
            foreach (Match match in matchesRglUrl)
            {
                if (index > RATECTRL)
                {
                    break;
                }
                foundSteamIDs.Add(match.Groups[1].ToString());
                index++;
            }
            foreach (Match match in matchesTf2centerUrl)
            {
                if (index > RATECTRL)
                {
                    break;
                }
                foundSteamIDs.Add(match.Groups[1].ToString());
                index++;
            }

            SteamIDs = foundSteamIDs;
        }

        public static string FormatToString(LeagueFormat lf)
        {
            string format = "";
            switch (lf)
            {
                case LeagueFormat.Sixes:
                    {
                        format = "6v6";
                        break;
                    }
                case LeagueFormat.HL:
                    {
                        format = "Highlander";
                        break;
                    }
                case LeagueFormat.PL:
                    {
                        format = "Prolander";
                        break;
                    }
                case LeagueFormat.NRSixes:
                    {
                        format = "No Restrictions 6v6";
                        break;
                    }
                case LeagueFormat.FourVeeFour:
                    {
                        format = "4v4";
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            return format;
        }

        /*
         * Debug stuff.
         */
        public void PrintIDs(Printer printer)
        {
            foreach (string steamID in SteamIDs)
            {
                printer(steamID);
            }
        }

    }
}
