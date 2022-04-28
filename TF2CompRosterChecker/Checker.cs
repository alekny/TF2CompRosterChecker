
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Xml;

namespace TF2CompRosterChecker
{
    abstract class Checker
    {
        private List<string> steamIDs;
        private string baseApiUrl;
        private string baseUrl;
        private string baseTeamUrl;
        public const int HL = 0;
        public const int Sixes = 1;
        public const int PL = 2;
        public const int NRSixes = 3;
        public const int FourVeeFour = 4;

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
                if (index > SteamIDTools.RATECTRL)
                {
                    break;
                }
                foundSteamIDs.Add(SteamIDTools.steamID3ToSteamID64(match.ToString()));
                index++;
            }
            foreach (Match match in matchesSteamID)
            {
                if (index > SteamIDTools.RATECTRL)
                {
                    break;
                }
                foundSteamIDs.Add(SteamIDTools.steamIDToSteamID64(match.ToString()));
                index++;
            }
            foreach (Match match in matchesProfileUrl)
            {
                if (index > SteamIDTools.RATECTRL)
                {
                    break;
                }
                foundSteamIDs.Add(match.Groups[1].ToString());
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
                            foundSteamIDs.Add(results.Item(0).InnerText);
                        }
                        index++;
                    }
                    catch (System.Net.WebException e)
                    {
                        // do nothing lul
                    }
                }
            }

            this.steamIDs = foundSteamIDs;
        }

        public abstract List<Player> ParseData(int leagueformat, ProgressBar progressBar, Button button);

        public List<string> SteamIDs
        {
            get { return this.steamIDs; }
            set { this.steamIDs = value; }
        }

        public string BaseApiUrl
        {
            get { return this.baseApiUrl; }
            set { this.baseApiUrl = value; }
        }
        public string BaseUrl
        {
            get { return this.baseUrl; }
            set { this.baseUrl = value; }
        }
        public string BaseTeamUrl
        {
            get { return this.baseTeamUrl; }
            set { this.baseTeamUrl = value; }
        }

        /*
         * Debug stuff.
         */
        public void printIDs()
        {
            foreach (string steamID in this.SteamIDS)
            {
                Console.WriteLine(steamID);
            }
        }

        public List<string> SteamIDS { get { return this.steamIDs; } }
    }
}
