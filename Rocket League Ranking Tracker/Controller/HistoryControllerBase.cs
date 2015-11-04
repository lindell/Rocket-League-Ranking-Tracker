using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Rocket_League_Ranking_Tracker.Controller
{
    public abstract class HistoryWindowControllerBase
    {
        public ControllerDataContext DataContext { get; set; }

        public abstract void ExportAsCsv();
        public abstract void DeleteItem(TableStruct selectedStruct);

        /// <summary>
        /// Struct for representing the data that is to be shown in the History Window
        /// </summary>
        public class TableStruct : INotifyPropertyChanged
        {
            private long _id;
            private int _rank;
            private DateTime _date;
            private int _viewIndex;

            public long Id { get { return _id; } set { _id = value; NotifyPropertyChanged("Id"); } }
            public int ViewIndex { get { return _viewIndex; } set { _viewIndex = value; NotifyPropertyChanged("ViewIndex");} }
            public int Rank { get { return _rank; } set { _rank = value; NotifyPropertyChanged("Rank"); } }
            public DateTime Date { get { return _date; } set { _date = value; NotifyPropertyChanged("Date"); } }

            public event PropertyChangedEventHandler PropertyChanged;

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
            public ObservableCollection<TableStruct> Entries { get; set; }
            public string Title { get; set; }
        }
    }
}
