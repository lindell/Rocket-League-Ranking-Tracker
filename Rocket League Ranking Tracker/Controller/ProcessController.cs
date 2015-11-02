using Rocket_League_Ranking_Tracker.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

namespace Rocket_League_Ranking_Tracker.Controller
{
    class ProcessController : INotifyPropertyChanged
    {
        private readonly BackgroundWorker _processWorker;

        private Process _rlProcess;
        private bool _searching = true;

        public Process RlProcess { get { return _rlProcess; } set { _rlProcess = value; NotifyPropertyChanged("Shots"); Searching=(value==null); } }
        public bool Searching { get { return _searching; } set { _searching = value; NotifyPropertyChanged("Searching"); } }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        List<IMemoryHandler> memoryHandlers = new List<IMemoryHandler>();

        public ProcessController()
        {
            _processWorker = new BackgroundWorker();
            _processWorker.DoWork += processWorker_DoWork;
            _processWorker.WorkerSupportsCancellation = true;
            _processWorker.RunWorkerAsync();
        }

        public void AddMemoryHandler(IMemoryHandler memoryHandler){ memoryHandlers.Add(memoryHandler); }
        public void RemoveMemoryHandler(IMemoryHandler memoryHandler) { memoryHandlers.Remove(memoryHandler); }

        private void processWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!_processWorker.CancellationPending)
            {
                RlProcess = GetProcessIfRunning();
                if (RlProcess != null)
                {
                    foreach (IMemoryHandler memoryHandler in memoryHandlers) {
                        memoryHandler.RocketLeagueProcess = RlProcess;
                    }
                }

                foreach (IMemoryHandler rm in memoryHandlers)
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
