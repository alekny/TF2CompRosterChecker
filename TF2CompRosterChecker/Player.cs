using System;

namespace TF2CompRosterChecker
{
    class Player
    {
        private string name;
        private string team;
        private string teamid;
        private string div;
        private string profileid;
        private string leagueid;
        private bool hasBans;

        public Player(string name, string team, string teamid, string div, string profileid, string leagueid, bool hasBans)
        {
            this.name = name;
            this.team = team;
            this.teamid = teamid;
            this.div = div;
            this.profileid = profileid;
            this.leagueid = leagueid;
            this.hasBans = hasBans;
        }

        public string Name { get { return this.name; } }
        public string Team { get { return this.team; } }
        public string Teamid { get { return this.teamid; } }
        public string Div { get { return this.div; } }
        public string Profileid { get { return this.profileid; } }
        public string Leagueid { get { return this.leagueid; } }
        public bool HasBans { get { return this.hasBans; } }

        public void print()
        {
            Console.WriteLine(string.Concat(name,string.Concat(",", string.Concat(team, string.Concat(",", div)))));
        }
    }    
}
