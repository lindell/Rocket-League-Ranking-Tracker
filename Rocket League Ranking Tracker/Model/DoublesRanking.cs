using System.Data.SQLite;
using Rocket_League_Ranking_Tracker.Model.Pointers;

namespace Rocket_League_Ranking_Tracker.Model
{
    class DoublesRanking : RankingModel
    {
        public DoublesRanking(SQLiteConnection con) : base(con)
        {
            RankingAddress = CheatEngineReader.getPointers("DOUBLE");
            Table = "DualsRanking";
        }
    }
}
