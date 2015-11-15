using System.Data.SQLite;

namespace Rocket_League_Ranking_Tracker.Model
{
    class SoloStandardRanking : RankingModel
    {
        public SoloStandardRanking(SQLiteConnection con) : base(con)
        {
            Address = "\"RocketLeague.exe\"+014FD4CC+124";
            Table = "SoloStandardRanking";
        }
    }
}
