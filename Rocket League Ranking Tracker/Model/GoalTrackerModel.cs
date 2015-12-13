using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Rocket_League_Ranking_Tracker.Model.Pointers;
using Rocket_League_Ranking_Tracker.Utilities.Memory;

namespace Rocket_League_Ranking_Tracker.Model
{
    public class GoalTrackerModel
    {
        protected string TimerAddress = "";
        protected string BlueGoalsAddress { get; set; }
        protected string OrangeGoalsAddress { get; set; }


        private int _orangeGoals = 0;
        public int OrangeGoal { get { return _orangeGoals; } set { _orangeGoals = value; NotifyPropertyChanged("OrangeGoals"); } }

        private int _blueGoals = 0;
        public int BlueGoal { get { return _blueGoals; } set { _blueGoals = value; NotifyPropertyChanged("BlueGoals"); } }

        public List<GoalMemoryStruct> Goals {get;}


        public event PropertyChangedEventHandler PropertyChanged;

        public GoalTrackerModel()
        {
            OrangeGoalsAddress = CheatEngineReader.getPointers("ORANGEGOALS");
            BlueGoalsAddress = CheatEngineReader.getPointers("BLUEGOALS");
            TimerAddress = CheatEngineReader.getPointers("TIMER");
            Goals = new List<GoalMemoryStruct>();
        }


        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public void UpdateMemory(Process process)
        {
            var memory = new Memory(process);
            int time = 0;
            int orangeGoals = 0;
            int blueGoals = 0;
            try
            {
                orangeGoals = memory.ReadInt32(memory.GetAddress(OrangeGoalsAddress));
                blueGoals = memory.ReadInt32(memory.GetAddress(BlueGoalsAddress));
                time = memory.ReadInt32(memory.GetAddress(TimerAddress));
            }
            catch (ArgumentException e)
            {
                if (e.Message == "Invalid address")
                {
                    Goals.Clear();
                    return;
                }
            }

            if (time > 600 || time < 0) return;
            if (orangeGoals != _orangeGoals)
            {
                Goals.Add(new GoalMemoryStruct()
                {
                    Team = TeamColor.Orange,
                    Time = memory.ReadInt32(memory.GetAddress(TimerAddress))
                });
                _orangeGoals = orangeGoals;
            }
            else if (blueGoals != _blueGoals) { 
                Goals.Add(new GoalMemoryStruct()
                {
                    Team = TeamColor.Blue,
                    Time = memory.ReadInt32(memory.GetAddress(TimerAddress))
                });
                _blueGoals = blueGoals;
            }
        }

        public enum TeamColor
        {
            Orange, Blue
        }

        //public class GoalStruct
        //{
        //    public int Time { get; set; }
        //    public TeamColor Team { get; set; }
        //}


        public enum Team
        {
            You, Opponent
        }

        public class GoalStruct
        {
            public int Time { get; set; }
            public Team Team { get; set; }
        }

        public class GoalMemoryStruct
        {
            public int Time { get; set; }
            public TeamColor Team { get; set; }
        }

        public void InsertGoals(SQLiteConnection dbConnection, string goalsTable,int gameId)
        {
            foreach (GoalMemoryStruct goal in Goals)
            {
                var query = $"INSERT into {goalsTable} (GameID ,Time, Team) values ({gameId}, {goal.Time}, '{goal.Team}')";
                var command = new SQLiteCommand(query, dbConnection);
                command.ExecuteNonQuery();
            }
            Goals.Clear();
        }
    }
}
