
using System.Collections.Generic;
using Xunit;
using Newtonsoft.Json;

namespace TF2CompRosterChecker.Tests
{
    public class ETF2LCheckerTests
    {
        [Fact]
        public static void ParseData_TestData()
        {
            List<Player> expected = new List<Player>();

            //Testuser set by ETF2L api documentation
            Player testplayer = new Player("testuser", "Test Team", "99999", "Low", "76561199999999999", "STEAM_0:0:99999999",
                "[U:1:99999999]", "99999", false, null);
            expected.Add(testplayer);

            ETF2LChecker ec = new ETF2LChecker("STEAM_0:0:99999999");
            //Compare against testdata
            List<Player> actual = ec.ParseData(Checker.LeagueFormat.Sixes, null);

            //Easier to compare
            Assert.Equal(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(actual));
        }
    }
}