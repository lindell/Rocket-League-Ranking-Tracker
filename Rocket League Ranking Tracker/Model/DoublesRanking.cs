using System.Data.SQLite;

namespace Rocket_League_Ranking_Tracker.Model
{
    class DoublesRanking : RankingModel
    {
        public DoublesRanking(SQLiteConnection con) : base(con)
        {
            Address = "\"RocketLeague.exe\"+0150354C+7c+48+80+50+18c";
            Table = "DualsRanking";
        }
    }
}
