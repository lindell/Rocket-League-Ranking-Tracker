using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocket_League_Ranking_Tracker.Model
{
    class DoublesRanking : RankingModel
    {
        public DoublesRanking(SQLiteConnection con) : base(con)
        {
            address = "\"RocketLeague.exe\"+015034C4+228+80+50+18c";
            table = "DualsRanking";
        }
    }
}
