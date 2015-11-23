using System.Data.SQLite;

namespace Rocket_League_Ranking_Tracker.Model
{
    class SoloRanking : RankingModel
    {
        public SoloRanking(SQLiteConnection con) : base(con)
        {
            Address = "\"RocketLeague.exe\" + 01557590 + 4a4+10+3c0+84";
            Table = "SoloRanking";
        }
    }
}
