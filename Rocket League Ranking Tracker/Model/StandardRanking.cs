using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;


namespace Rocket_League_Ranking_Tracker.Model
{
    class StandardRanking : RankingModel
    {
        public StandardRanking(SQLiteConnection con) : base(con)
        {
            address = "\"RocketLeague.exe\"+015034C4+228+80+70+18c";
            table = "StandardRanking";
        }
    }
}
