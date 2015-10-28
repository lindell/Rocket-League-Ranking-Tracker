using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rocket_League_Ranking_Tracker.Utilities.Memory;
using System.Diagnostics;
using System.ComponentModel;

namespace Rocket_League_Ranking_Tracker.Model
{
    class RankingModel : INotifyPropertyChanged
    {
        protected string address = "";
        int _Ranking;
        public RankingModel()
        {

        }

        public int Ranking{ get { return _Ranking; } set { _Ranking = value; NotifyPropertyChanged("Ranking"); } }

        public Process RocketLeagueProcess { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public void updateRanking()
        {
            if (RocketLeagueProcess != null) { 
                var memory = new Memory(RocketLeagueProcess);
                IntPtr rankingAddr = memory.GetAddress(address);

                var ranking = memory.ReadInt32(rankingAddr);
                Ranking = ranking;
            }
        }
    }
}
