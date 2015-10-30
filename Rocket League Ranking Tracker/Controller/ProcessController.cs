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
    class ProcessController : INotifyPropertyChanged
    {
        private readonly BackgroundWorker _processWorker;

        private Process _RLProcess;
        private bool searching = true;

        public Process RLProcess { get { return _RLProcess; } set { _RLProcess = value; NotifyPropertyChanged("Shots"); Searching=(value==null); } }
        public bool Searching { get { return searching; } set { searching = value; NotifyPropertyChanged("Searching"); } }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        List<MemoryHandler> memoryHandlers = new List<MemoryHandler>();

        public ProcessController()
        {
            _processWorker = new BackgroundWorker();
            _processWorker.DoWork += processWorker_DoWork;
            _processWorker.WorkerSupportsCancellation = true;
            _processWorker.RunWorkerAsync();
        }

        public void AddMemoryHandler(MemoryHandler memoryHandler){ memoryHandlers.Add(memoryHandler); }
        public void RemoveMemoryHandler(MemoryHandler memoryHandler) { memoryHandlers.Remove(memoryHandler); }

        private void processWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!_processWorker.CancellationPending)
            {
                RLProcess = GetProcessIfRunning();
                if (RLProcess != null)
                {
                    foreach (MemoryHandler memoryHandler in memoryHandlers) {
                        memoryHandler.RocketLeagueProcess = RLProcess;
                    }
                }

                foreach (MemoryHandler rm in memoryHandlers)
                {
                    rm.UpdateMemory();
                }

                Thread.Sleep(3000);
            }
        }

        public Process GetProcessIfRunning()
        {
            var rlProcess = Process.GetProcessesByName("RocketLeague");
            return rlProcess.Length >= 1 ? rlProcess[0] : null;
        }
    }
}
