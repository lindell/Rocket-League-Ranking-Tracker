using System.Data.SQLite;

namespace Rocket_League_Ranking_Tracker.Model
{
    class SoloRanking : RankingModel
    {
        public SoloRanking(SQLiteConnection con) : base(con)
        {
            Address = "\"RocketLeague.exe\"+0150354C+7c+48+80+40+18c";
            Table = "SoloRanking";
        }
    }
}
