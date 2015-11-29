using System.Data.SQLite;

namespace Rocket_League_Ranking_Tracker.Model
{
    class DoublesRanking : RankingModel
    {
        public DoublesRanking(SQLiteConnection con) : base(con)
        {
            Address = "\"RocketLeague.exe\" + 014F9C5C + 108 + 244 + 680 + 1c4";
            Table = "DualsRanking";
        }
    }
}
