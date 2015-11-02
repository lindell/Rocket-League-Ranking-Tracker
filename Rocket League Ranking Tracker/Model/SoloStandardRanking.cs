using System.Data.SQLite;

namespace Rocket_League_Ranking_Tracker.Model
{
    class SoloStandardRanking : RankingModel
    {
        public SoloStandardRanking(SQLiteConnection con) : base(con)
        {
            Address = "\"RocketLeague.exe\"+0150354C+7c+48+80+60+18c";
            Table = "SoloStandardRanking";
        }
    }
}
