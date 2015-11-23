using System.Data.SQLite;

namespace Rocket_League_Ranking_Tracker.Model
{
    class SoloStandardRanking : RankingModel
    {
        public SoloStandardRanking(SQLiteConnection con) : base(con)
        {
            Address = "\"RocketLeague.exe\" + 01557590 + 4a4+10+2a0+84";
            Table = "SoloStandardRanking";
        }
    }
}
