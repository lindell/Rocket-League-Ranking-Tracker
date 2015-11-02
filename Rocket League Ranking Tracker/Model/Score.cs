using Rocket_League_Ranking_Tracker.Utilities.Memory;
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Rocket_League_Ranking_Tracker.Model
{
    class Score : INotifyPropertyChanged,IMemoryHandler
    {
        private string baseAddress = "\"RocketLeague.exe\"+014FE8F8+4f0+178+55c+1a4";
        //private string baseAddress = "\"RocketLeague.exe\"+014FE8F8+4f0+178+55c+1d4";
        private readonly string _shotsAddress;
        private readonly string _savesAddress;
        private readonly string _goalsAddress;
        private readonly string _pointsAddress;
        private readonly string _assistsAddress;

        private int _shotsOnGoal;
        private int _saves;
        private int _goals;
        private int _points;
        private int _assists;

        public Score()
        {
            _shotsAddress = baseAddress + "+84";
            _savesAddress = baseAddress + "+80";
            _goalsAddress = baseAddress + "+78";
            _pointsAddress = baseAddress + "+74";
            _assistsAddress = baseAddress + "+7C";
        }

        public int Shots { get { return _shotsOnGoal; } set { _shotsOnGoal = value; NotifyPropertyChanged("Shots"); } }
        public int Saves { get { return _saves; } set { _saves = value; NotifyPropertyChanged("Saves"); } }
        public int Goals { get { return _goals; } set { _goals = value; NotifyPropertyChanged("Goals"); } }
        public int Points { get { return _points; } set { _points = value; NotifyPropertyChanged("Points"); } }
        public int Assists { get { return _assists; } set { _assists = value; NotifyPropertyChanged("Assists"); } }

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

                Shots = memory.ReadInt32(memory.GetAddress(_shotsAddress));
                Saves = memory.ReadInt32(memory.GetAddress(_savesAddress));
                Goals = memory.ReadInt32(memory.GetAddress(_goalsAddress));
                Points = memory.ReadInt32(memory.GetAddress(_pointsAddress));
                Assists = memory.ReadInt32(memory.GetAddress(_assistsAddress));
            }
        }
    }
}
