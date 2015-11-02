using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rocket_League_Ranking_Tracker.Utilities.Memory;
using System.Diagnostics;
using System.ComponentModel;
using System.Data.SQLite;

namespace Rocket_League_Ranking_Tracker.Model
{
    class RankingModel : INotifyPropertyChanged, MemoryHandler
    {
        protected string address = "";
        protected string table = "";

        int _Ranking;
        protected SQLiteConnection dbConnection;

        public RankingModel(SQLiteConnection dbConnection)
        {
            this.dbConnection = dbConnection;
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

        public void UpdateMemory()
        {
            if (RocketLeagueProcess != null)
            {
                var memory = new Memory(RocketLeagueProcess);
                IntPtr rankingAddr = memory.GetAddress(address);

                var ranking = memory.ReadInt32(rankingAddr);
                Ranking = ranking;
                if(ranking != GetPreviousRanking() && ranking != 50 && ranking !=0 && ranking > 0 && ranking < 2000)
                    //TODO: remove magic constants
                {
                    UpdatePreviousRanking(ranking);
                }
            }
        }

        private void UpdatePreviousRanking(int ranking)
        {
            string query = "INSERT into " + table + "(Rank, Date) values (" + ranking + ", '" + DateTime.Now + "')";
            SQLiteCommand command = new SQLiteCommand(query, dbConnection);
            command.ExecuteNonQuery();
        }

        private int GetPreviousRanking()
        {
            string query = "SELECT Rank FROM " + table + " ORDER BY Date DESC LIMIT 1";
            SQLiteCommand command = new SQLiteCommand(query, dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                if ((int)reader["Rank"] != 0)
                    return (int)reader["Rank"];
            }
            return 0;
        }
    }
}
