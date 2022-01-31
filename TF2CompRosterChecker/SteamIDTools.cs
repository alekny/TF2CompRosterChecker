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

using System;
using System.Net;
using System.Text.RegularExpressions;

namespace TF2CompRosterChecker
{
    class SteamIDTools
    {
        public const string steamIDregex = @"STEAM_[0-5]:[01]:[0-9]+";
        public const string steamID3regex = @"\[U:1:[0-9]+\]";
        public const string baseUrl = "https://steamcommunity.com/profiles/";
        public const string baseLogsUrl = "https://logs.tf/profile/";
        public const string profileUrlregex = @"(?:https?:\/\/)?steamcommunity\.com\/profiles\/([0-9]{17})(?:\/?)";
        public const string profileCustomUrlregex = @"(?:https?:\/\/)?steamcommunity\.com\/id\/[a-zA-Z0-9_-]+";
        public const string idFinderRegex = "\"steamid\":\"([0-9]+)\"";
        public const string profileCustomUrlXml = "https://steamcommunity.com/id/";
        public const int RATECTRL = 50;


        public static string steamIDToSteamID64(string steamID)
        {
            //long just for adding some numbers? hmm...
            long steamID64 = 76561197960265728;
            string[] args = steamID.Split(':');
            steamID64 += Int32.Parse(args[2]) * 2;
            if ("1".Equals(args[1]))
            {
                steamID64 += 1;
            }
            return Convert.ToString(steamID64);
            
        }

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
            return "[U:1:" + (Int64.Parse(steamID64) - 76561197960265728) + "]";
        }

        public static string steamID64ToSteamID(string steamID64)
        {
            Int64 accid = Int64.Parse(steamID64) - 76561197960265728;
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
