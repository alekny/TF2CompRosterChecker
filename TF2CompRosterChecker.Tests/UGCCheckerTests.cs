using System.Collections.Generic;
using TF2CompRosterChecker;
using Xunit;
using Newtonsoft.Json;

namespace TF2CompRosterChecker.Tests
{
    public class UGCCheckerTests
    {
        [Fact]
        public static void ParseData_TestData()
        {
            List<Player> expected = new List<Player>();

            //Doesn't work here, need to use another Test Account
            Player testplayer = new Player("testuser", "Test Team", "99999", "Low", "76561199999999999", "STEAM_0:0:99999999",
                "[U:1:99999999]", "99999", false, null);
            expected.Add(testplayer);

            UGCChecker uc = new UGCChecker("STEAM_0:0:99999999");
            //Compare against testdata
            uc.BaseApiUrl = "https://proph.im/testdata/";
            List<Player> actual = uc.ParseData(Checker.LeagueFormat.Sixes, null);

            //Easier to compare
            Assert.Equal(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(actual));
        }
    }
}
