
using System.Collections.Generic;
using System.ComponentModel;

namespace TF2CompRosterChecker
{
    public sealed class Player
    {
        //Do we want the output inside the TextBox or in console?
        public delegate void Printer(string message);

        public Player(string name, string team, string teamid, string div, string profileid, 
            string steamid, string steamid3, string leagueid, bool hasBans, List<Ban> bans)
        {
            Name = name;
            Team = team;
            Teamid = teamid;
            Div = div;
            Profileid = profileid;
            Steamid = steamid;
            Steamid3 = steamid3;
            Leagueid = leagueid;
            HasBans = hasBans;
            Bans = bans;
        }

        public string Name { get; }
        public string Team { get; }
        public string Teamid { get; }
        public string Div { get; }
        public string Profileid { get; }
        public string Steamid { get; }
        public string Steamid3 { get; }
        public string Leagueid { get; }
        public bool HasBans { get; }
        public List<Ban> Bans { get; } = null;

        public void Print(Printer printer)
        {
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(this))
            {
                string name = descriptor.Name;
                object value = descriptor.GetValue(this);
                printer(name + ", " + value + "\n");
            }
        }
    }

    public sealed class Ban
    {
        public Ban(string start, string end, string reason)
        {
            Start = start;
            End = end;
            Reason = reason;
        }

        public string Start { get; }
        public string End { get; }
        public string Reason { get; }
    }
}
