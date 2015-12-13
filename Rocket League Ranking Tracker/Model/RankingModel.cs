using System;
using Rocket_League_Ranking_Tracker.Utilities.Memory;
using System.Diagnostics;
using System.ComponentModel;
using System.Data.SQLite;
using Rocket_League_Ranking_Tracker.Model.Pointers;

namespace Rocket_League_Ranking_Tracker.Model
{
    class RankingModel : INotifyPropertyChanged, IMemoryHandler
    {
        protected string RankingAddress = "";
        protected string Table = "";
        protected string OrangeGoalsAddress;
        protected string BlueGoalsAddress;
        protected string GoalsTable;

        private int _ranking;
        protected SQLiteConnection DbConnection;
        private Process _process;
        private GoalTrackerModel _goalTracker;
        

        public RankingModel(SQLiteConnection dbConnection)
        {
            DbConnection = dbConnection;
            OrangeGoalsAddress = CheatEngineReader.getPointers("ORANGEGOALS");
            BlueGoalsAddress = CheatEngineReader.getPointers("BLUEGOALS");
            _goalTracker = new GoalTrackerModel();
        }

        public int Ranking{ get { return _ranking; } set { _ranking = value; NotifyPropertyChanged("Ranking"); } }

        public Process RocketLeagueProcess {get; set; } 

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
                _goalTracker.UpdateMemory(RocketLeagueProcess);
                var memory = new Memory(RocketLeagueProcess);
                IntPtr rankingAddr = memory.GetAddress(RankingAddress);
                var ranking = memory.ReadInt32(rankingAddr);
                Ranking = ranking;
                if(ranking != GetPreviousRanking() && ranking != 50 && ranking !=0 && ranking > 0 && ranking < 2000)
                    //TODO: remove magic constants
                {
                    UpdatePreviousRanking(ranking);
                    _goalTracker.InsertGoals(DbConnection,GoalsTable, GetPreviousId());
                }
            }
        }

        private int GetPreviousId()
        {
            string query = $"SELECT Id FROM {Table} ORDER BY Date DESC LIMIT 1";
            var command = new SQLiteCommand(query, DbConnection);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                if ((long)reader["Id"] != 0)
                    return (int)(long)reader["Id"];
            }
            return 0;
        }

        private void UpdatePreviousRanking(int ranking)
        {
            var memory = new Memory(RocketLeagueProcess);
            int orangeGoals = 0;
            int blueGoals = 0;
            try
            {
                orangeGoals = memory.ReadInt32(memory.GetAddress(OrangeGoalsAddress));
                blueGoals = memory.ReadInt32(memory.GetAddress(BlueGoalsAddress));
            }
            catch (ArgumentException e)
            {
                if (e.Message != "Invalid address")
                    throw e;
            }



            var query = $"INSERT into {Table} (Rank, Date, OrangeGoals, BlueGoals) values ({ranking} , '{DateTime.Now}', {orangeGoals}, {blueGoals})";
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
