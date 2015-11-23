using System.Data.SQLite;


namespace Rocket_League_Ranking_Tracker.Model
{
    class StandardRanking : RankingModel
    {
        public StandardRanking(SQLiteConnection con) : base(con)
        {
            Address = "\"RocketLeague.exe\" + 01557590 + 4a4+10+260+84";
            Table = "StandardRanking";
        }
    }
}
