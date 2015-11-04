using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Microsoft.Windows.Controls;
using DataGrid = System.Windows.Controls.DataGrid;

namespace Rocket_League_Ranking_Tracker.Controller
{
    public abstract class HistoryWindowControllerBase
    {
        protected ArrayList EntriesToRemove;
        protected ArrayList EntriesToUpdate;
        public ControllerDataContext DataContext { get; set; }

        private ObservableCollection<TableStruct> _entries; 

        public abstract void ExportAsCsv();
        public abstract void DeleteItem(DataGrid dataGrid);

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


        public class ControllerDataContext
        {
            public ObservableCollection<TableStruct> Entries { get; set; }
            public string Title { get; set; }
        }

        public abstract void PreviewKeyDown(object sender, KeyEventArgs keyEventArgs);
    }
}
