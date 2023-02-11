
using System;
using System.Net;
using System.Text.RegularExpressions;

namespace TF2CompRosterChecker
{
    internal sealed class SteamIDTools
    {
        public const string steamIDregex = @"STEAM_[0-5]:[01]:[0-9]{1,9}";
        public const string steamID3regex = @"U:1:[0-9]{1,10}";
        public const string baseUrl = "https://steamcommunity.com/profiles/";
        public const string baseLogsUrl = "https://logs.tf/profile/";
        public const string profileUrlregex = @"(?:https?:\/\/)?steamcommunity\.com\/profiles\/([0-9]{17})(?:\/?)";
        public const string profileCustomUrlregex = @"(?:https?:\/\/)?steamcommunity\.com\/id\/[a-zA-Z0-9_-]{1,50}";
        public const string idFinderRegex = "\"steamid\":\"([0-9]+)\"";
        public const string profileCustomUrlXml = "https://steamcommunity.com/id/";
        public const string etf2lProfileUrl = @"(?:https?:\/\/)?etf2l.org\/forum\/user\/([0-9]{1,9})(?:\/?)";
        public const string ugcProfileUrl = @"(?:https?:\/\/)?ugcleague\.com\/players_page\.cfm\?player_id=([0-9]{17})(?:\/?)";
        public const string rglProfileUrl = @"(?:https?:\/\/)?rgl.gg\/Public\/PlayerProfile\.aspx\?p=([0-9]{17})(?:\/?)";
        public const string urlRegex = @"https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)";

        public static string SteamIDToSteamID64(string steamID)
        {
            //long just for adding some numbers? hmm...
            long steamID64 = 76561197960265728;
            string[] args = steamID.Split(':');
            steamID64 += int.Parse(args[2]) * 2;
            if ("1".Equals(args[1]))
            {
                steamID64 += 1;
            }
            return Convert.ToString(steamID64); 
        }

        public static string SteamID3ToSteamID64(string steamID3)
        {
            string[] args = steamID3.Split(':');
            
            int accid = int.Parse(args[2].Substring(0, args[2].Length));
            int Y, Z;
            if ((accid % 2) == 0)
            {
                Y = 0;
                Z = accid / 2;
            }
            else
            {
                Y = 1;
                Z = (accid - 1) / 2;
            }
            return "7656119" + Convert.ToString((Z * 2) + (7960265728 + Y));
        }

        public static string GetSteamID64FromCustomUrl(string customUrl)
        {
            using (WebClient wc = new WebClient())
            {
                string dl = "";
                try
                {
                    dl = wc.DownloadString(customUrl);
                }
                catch (WebException)
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

        public static string SteamID64ToSteamID3(string steamID64)
        {
            return "[U:1:" + (long.Parse(steamID64) - 76561197960265728) + "]";
        }

        public static string SteamID64ToSteamID(string steamID64)
        {
            long accid = long.Parse(steamID64) - 76561197960265728;
            int parity = 0;
            if (accid % 2 == 0)
            {
                accid /= 2;
            }
            else
            {
                parity = 1;
                accid = (accid - 1) / 2;
            }
            return "STEAM_0:" + parity + ":" + accid;
        }
    }
}
