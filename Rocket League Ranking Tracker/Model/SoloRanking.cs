using System.Data.SQLite;
using Rocket_League_Ranking_Tracker.Model.Pointers;

namespace Rocket_League_Ranking_Tracker.Model
{
    class SoloRanking : RankingModel
    {
        public SoloRanking(SQLiteConnection con) : base(con)
        {
            RankingAddress = CheatEngineReader.getPointers("SOLO");
            Table = "SoloRanking";
            GoalsTable = "SoloGoals";

        }
    }
}
