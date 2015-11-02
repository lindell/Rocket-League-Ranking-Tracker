﻿using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocket_League_Ranking_Tracker.Model
{
    class SoloStandardRanking : RankingModel
    {
        public SoloStandardRanking(SQLiteConnection con) : base(con)
        {
            address = "\"RocketLeague.exe\"+0150354C+7c+48+80+60+18c";
            table = "SoloStandardRanking";
        }
    }
}
