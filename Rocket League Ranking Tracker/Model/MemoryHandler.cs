using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocket_League_Ranking_Tracker.Model
{
    interface MemoryHandler
    {
        void UpdateMemory();
        Process RocketLeagueProcess { get; set; }
    }
}
