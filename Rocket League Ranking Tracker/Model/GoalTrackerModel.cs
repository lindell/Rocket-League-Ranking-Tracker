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
    class GoalTrackerModel
    {
        protected string TimerAddress = "";
        protected string BlueGoalsAddress { get; set; }
        protected string OrangeGoalsAddress { get; set; }


        private int _orangeGoals = 0;
        public int OrangeGoal { get { return _orangeGoals; } set { _orangeGoals = value; NotifyPropertyChanged("OrangeGoals"); } }

        private int _blueGoals = 0;
        public int BlueGoal { get { return _blueGoals; } set { _blueGoals = value; NotifyPropertyChanged("BlueGoals"); } }

        public List<GoalStruct> Goals {get;}


        public event PropertyChangedEventHandler PropertyChanged;

        public GoalTrackerModel()
        {
            OrangeGoalsAddress = CheatEngineReader.getPointers("ORANGEGOALS");
            BlueGoalsAddress = CheatEngineReader.getPointers("BLUEGOALS");
            TimerAddress = CheatEngineReader.getPointers("TIMER");
            Goals = new List<GoalStruct>();
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
                int time;
                try
                {
                    var address = memory.GetAddress(TimerAddress);
                    var tmp = memory.ReadInt32(memory.GetAddress(TimerAddress));
                }
                catch (ArgumentException e)
                {
                    if (e.Message == "Invalid address")
                    {
                        Goals.Clear();
                        return;
                    }
                }
                time = memory.ReadInt32(memory.GetAddress(TimerAddress));
                if (time > 600 || time < 0) return;
                var orangeGoals = memory.ReadInt32(memory.GetAddress(OrangeGoalsAddress));
                var blueGoals = memory.ReadInt32(memory.GetAddress(BlueGoalsAddress));

                if (orangeGoals != _orangeGoals)
                {
                    Goals.Add(new GoalStruct()
                    {
                        Team = TeamColor.Orange,
                        Time = memory.ReadInt32(memory.GetAddress(TimerAddress))
                    });
                    _orangeGoals = orangeGoals;
                }
                else if (blueGoals != _blueGoals) { 
                    Goals.Add(new GoalStruct()
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

        public class GoalStruct
        {
            public int Time { get; set; }
            public TeamColor Team { get; set; }
        }

        public void InsertGoals(SQLiteConnection dbConnection, string goalsTable,int gameId)
        {
            foreach (GoalStruct goal in Goals)
            {
                var query = $"INSERT into {goalsTable} (GameID ,Time, Team) values ({gameId}, {goal.Time}, '{goal.Team}')";
                var command = new SQLiteCommand(query, dbConnection);
                command.ExecuteNonQuery();
            }
            Goals.Clear();
        }
    }
}
