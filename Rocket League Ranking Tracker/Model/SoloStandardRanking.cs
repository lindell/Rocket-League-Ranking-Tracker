using System.Data.SQLite;

namespace Rocket_League_Ranking_Tracker.Model
{
    class SoloStandardRanking : RankingModel
    {
        public SoloStandardRanking(SQLiteConnection con) : base(con)
        {
            Address = "\"RocketLeague.exe\" + 014F9C5C + 108 + 244 + 640 + 1c4";
            Table = "SoloStandardRanking";
        }
    }
}
