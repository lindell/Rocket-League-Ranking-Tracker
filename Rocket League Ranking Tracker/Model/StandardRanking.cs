using System.Data.SQLite;


namespace Rocket_League_Ranking_Tracker.Model
{
    class StandardRanking : RankingModel
    {
        public StandardRanking(SQLiteConnection con) : base(con)
        {
            Address = "\"RocketLeague.exe\" + 014F9C5C + 108 + 244 + 580 + 1c4";
            Table = "StandardRanking";
        }
    }
}
