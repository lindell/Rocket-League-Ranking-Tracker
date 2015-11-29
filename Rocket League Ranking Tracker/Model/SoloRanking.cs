using System.Data.SQLite;

namespace Rocket_League_Ranking_Tracker.Model
{
    class SoloRanking : RankingModel
    {
        public SoloRanking(SQLiteConnection con) : base(con)
        {
            Address = "\"RocketLeague.exe\" + 014F9C5C + 108 + 244 + 720 + 1c4";
            Table = "SoloRanking";
        }
    }
}
