using System.Data.SQLite;


namespace Rocket_League_Ranking_Tracker.Model
{
    class StandardRanking : RankingModel
    {
        public StandardRanking(SQLiteConnection con) : base(con)
        {
            Address = "\"RocketLeague.exe\"+014FD4CC+84";
            Table = "StandardRanking";
        }
    }
}
