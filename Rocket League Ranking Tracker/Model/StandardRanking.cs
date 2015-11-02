using System.Data.SQLite;


namespace Rocket_League_Ranking_Tracker.Model
{
    class StandardRanking : RankingModel
    {
        public StandardRanking(SQLiteConnection con) : base(con)
        {
            Address = "\"RocketLeague.exe\"+0150354C+7c+48+80+70+18c";
            Table = "StandardRanking";
        }
    }
}
