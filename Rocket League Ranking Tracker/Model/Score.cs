using Rocket_League_Ranking_Tracker.Utilities.Memory;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocket_League_Ranking_Tracker.Model
{
    class Score : INotifyPropertyChanged,MemoryHandler
    {
        private string baseAddress = "\"RocketLeague.exe\"+014FE8F8+4f0+178+55c+1a4";
        //private string baseAddress = "\"RocketLeague.exe\"+014FE8F8+4f0+178+55c+1d4";
        private string shotsAddress;
        private string savesAddress;
        private string goalsAddress;
        private string pointsAddress;
        private string assistsAddress;

        private int _ShotsOnGoal;
        private int _Saves;
        private int _Goals;
        private int _Points;
        private int _Assists;

        public Score()
        {
            shotsAddress = baseAddress + "+84";
            savesAddress = baseAddress + "+80";
            goalsAddress = baseAddress + "+78";
            pointsAddress = baseAddress + "+74";
            assistsAddress = baseAddress + "+7C";
        }

        public int Shots { get { return _ShotsOnGoal; } set { _ShotsOnGoal = value; NotifyPropertyChanged("Shots"); } }
        public int Saves { get { return _Saves; } set { _Saves = value; NotifyPropertyChanged("Saves"); } }
        public int Goals { get { return _Goals; } set { _Goals = value; NotifyPropertyChanged("Goals"); } }
        public int Points { get { return _Points; } set { _Points = value; NotifyPropertyChanged("Points"); } }
        public int Assists { get { return _Assists; } set { _Assists = value; NotifyPropertyChanged("Assists"); } }

        public Process RocketLeagueProcess { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public void UpdateMemory()
        {
            if (RocketLeagueProcess != null)
            {
                var memory = new Memory(RocketLeagueProcess);

                Shots = memory.ReadInt32(memory.GetAddress(shotsAddress));
                Saves = memory.ReadInt32(memory.GetAddress(savesAddress));
                Goals = memory.ReadInt32(memory.GetAddress(goalsAddress));
                Points = memory.ReadInt32(memory.GetAddress(pointsAddress));
                Assists = memory.ReadInt32(memory.GetAddress(assistsAddress));
            }
        }
    }
}
