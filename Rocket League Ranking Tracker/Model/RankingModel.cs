using System;
using Rocket_League_Ranking_Tracker.Utilities.Memory;
using System.Diagnostics;
using System.ComponentModel;
using System.Data.SQLite;

namespace Rocket_League_Ranking_Tracker.Model
{
    class RankingModel : INotifyPropertyChanged, IMemoryHandler
    {
        protected string Address = "";
        protected string Table = "";

        private int _ranking;

        protected SQLiteConnection DbConnection;

        public RankingModel(SQLiteConnection dbConnection)
        {
            DbConnection = dbConnection;
        }

        public int Ranking{ get { return _ranking; } set { _ranking = value; NotifyPropertyChanged("Ranking"); } }

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
                IntPtr rankingAddr = memory.GetAddress(Address);

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
            var query = $"INSERT into {Table} (Rank, Date) values ({ranking} , '{DateTime.Now}')";
            var command = new SQLiteCommand(query, DbConnection);
            command.ExecuteNonQuery();
        }

        private int GetPreviousRanking()
        {
            string query = $"SELECT Rank FROM {Table} ORDER BY Date DESC LIMIT 1";
            var command = new SQLiteCommand(query, DbConnection);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                if ((int)reader["Rank"] != 0)
                    return (int)reader["Rank"];
            }
            return 0;
        }
    }
}
