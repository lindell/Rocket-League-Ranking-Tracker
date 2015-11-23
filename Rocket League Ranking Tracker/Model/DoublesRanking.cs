using System.Data.SQLite;

namespace Rocket_League_Ranking_Tracker.Model
{
    class DoublesRanking : RankingModel
    {
        public DoublesRanking(SQLiteConnection con) : base(con)
        {
            Address = "\"RocketLeague.exe\" + 01557590 + 4a4+10+340+84";
            Table = "DualsRanking";
        }
    }
}
