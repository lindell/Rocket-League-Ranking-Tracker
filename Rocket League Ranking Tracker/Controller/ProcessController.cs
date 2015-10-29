using Rocket_League_Ranking_Tracker.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rocket_League_Ranking_Tracker.Controller
{
    class ProcessController
    {
        private readonly BackgroundWorker _processWorker;

        List<MemoryHandler> rankingModels = new List<MemoryHandler>();

        public ProcessController()
        {
            _processWorker = new BackgroundWorker();
            _processWorker.DoWork += processWorker_DoWork;
            _processWorker.WorkerSupportsCancellation = true;
            _processWorker.RunWorkerAsync();
        }

        public void AddMemoryHandler(MemoryHandler rankingModel){ rankingModels.Add(rankingModel); }
        public void RemoveMemoryHandler(MemoryHandler rankingModel){ rankingModels.Remove(rankingModel); }

        private void processWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!_processWorker.CancellationPending)
            {
                Thread.Sleep(3000);

                Process rlProcess = GetProcessIfRunning();
                if (rlProcess != null)
                {
                    foreach (MemoryHandler rm in rankingModels) { 
                        rm.RocketLeagueProcess = rlProcess;
                    }
                }

                foreach (MemoryHandler rm in rankingModels)
                {
                    rm.UpdateMemory();
                }
            }
        }

        public Process GetProcessIfRunning()
        {
            var rlProcess = Process.GetProcessesByName("RocketLeague");
            return rlProcess.Length >= 1 ? rlProcess[0] : null;
        }
    }
}
