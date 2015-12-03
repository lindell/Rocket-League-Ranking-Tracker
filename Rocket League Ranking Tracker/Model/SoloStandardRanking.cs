using System.Data.SQLite;
using Rocket_League_Ranking_Tracker.Model.Pointers;

namespace Rocket_League_Ranking_Tracker.Model
{
    class SoloStandardRanking : RankingModel
    {
        public SoloStandardRanking(SQLiteConnection con) : base(con)
        {
            Address = CheatEngineReader.getPointers("SOLOSTANDARD");
            Table = "SoloStandardRanking";
        }
    }
}
