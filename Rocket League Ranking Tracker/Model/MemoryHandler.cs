using System.Diagnostics;

namespace Rocket_League_Ranking_Tracker.Model
{
    interface IMemoryHandler
    {
        void UpdateMemory();
        Process RocketLeagueProcess { get; set; }
    }
}
