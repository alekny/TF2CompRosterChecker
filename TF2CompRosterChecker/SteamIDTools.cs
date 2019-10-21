using System;
using System.Net;
using System.Text.RegularExpressions;

namespace TF2CompRosterChecker
{
    class SteamIDTools
    {
        public static string steamID3regex = @"\[U:1:[0-9]+\]";
        public static string baseUrl = "https://steamcommunity.com/profiles/";
        public static string baseLogsUrl = "https://logs.tf/profile/";
        public static string profileUrlregex = @"(?:https?:\/\/)?steamcommunity\.com\/profiles\/([0-9]{17})(?:\/?)";
        public static string profileCustomUrlregex = @"(?:https?:\/\/)?steamcommunity\.com\/id\/[a-zA-Z0-9]+\/";
        public static string idFinderRegex = "\"steamid\":\"([0-9]+)\"";

        public static string steamID3ToSteamID64(string steamID3)
        {
            string[] args = steamID3.Split(':');
            int accid = Int32.Parse(args[2].Substring(0, args[2].Length - 1));
            int Y, Z = 0;
            if ((accid % 2) == 0)
            {
                Y = 0;
                Z = (accid / 2);
            }
            else
            {
                Y = 1;
                Z = ((accid - 1) / 2);
            }
            return string.Concat("7656119", Convert.ToString((Z * 2) + (7960265728 + Y)));
        }

        public static string getSteamID64FromCustomUrl(string customUrl)
        {
            using (WebClient wc = new WebClient())
            {
                string dl = "";
                try
                {
                    dl = wc.DownloadString(customUrl);
                }
                catch (System.Net.WebException e)
                {
                    return null;
                }
                Match match = Regex.Match(dl, SteamIDTools.idFinderRegex);
                if (match != null && match.Length != 0)
                {
                    return match.Groups[1].ToString();
                }
                return "[ERROR]";
            }
        }

        public static string steamID64ToSteamID3(string steamID64)
        {
            throw new NotImplementedException();
        }
    }
}
