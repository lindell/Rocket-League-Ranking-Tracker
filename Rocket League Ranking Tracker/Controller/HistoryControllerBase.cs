using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Rocket_League_Ranking_Tracker.Model;

namespace Rocket_League_Ranking_Tracker.Controller
{
    public abstract class HistoryWindowControllerBase
    {
        public ControllerDataContext DataContext { get; set; }

        public abstract void ExportAsCsv();
        public abstract void DeleteItem(RankTableStruct selectedStruct);

        /// <summary>
        /// Struct for representing the data that is to be shown in the History Window
        /// </summary>
        public class RankTableStruct : INotifyPropertyChanged
        {
            private long _id;
            private int _rank;
            private DateTime _date;
            private int _viewIndex;
            private int _goalDifference;

            public long Id { get { return _id; } set { _id = value; NotifyPropertyChanged("Id"); } }
            public int ViewIndex { get { return _viewIndex; } set { _viewIndex = value; NotifyPropertyChanged("ViewIndex");} }
            public int Rank { get { return _rank; } set { _rank = value; NotifyPropertyChanged("Rank"); } }
            public DateTime Date { get { return _date; } set { _date = value; NotifyPropertyChanged("Date"); } }
            public int GoalDifference { get { return _goalDifference; } set { _goalDifference = value; NotifyPropertyChanged("GoalDifference"); } }
            public ObservableCollection<GoalTrackerModel.GoalStruct> Goals { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;

            public RankTableStruct()
            {
                Goals = new ObservableCollection<GoalTrackerModel.GoalStruct>();
            }
            private void NotifyPropertyChanged(String info)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
                }
            }

        }


        /// <summary>
        /// The dataContext struct to be used in the HistoryWindow
        /// </summary>
        public class ControllerDataContext
        {
            public ObservableCollection<RankTableStruct> RankEntries { get; set; }
            public string Title { get; set; }
        }
    }
}
