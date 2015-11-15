using System.Data.SQLite;

namespace Rocket_League_Ranking_Tracker.Model
{
    class DoublesRanking : RankingModel
    {
        public DoublesRanking(SQLiteConnection con) : base(con)
        {
            Address = "\"RocketLeague.exe\"+014FD4E4+84";
            Table = "DualsRanking";
        }
    }
}
