﻿using Rocket_League_Ranking_Tracker.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocket_League_Ranking_Tracker.Controller
{
    public abstract class HistoryWindowControllerBase
    {
        protected ArrayList entriesToRemove;
        protected ArrayList entriesToUpdate;

        public ObservableCollection<TableStruct> Entries { get; set; }

        public abstract void ExportToExcel();
        public abstract void ExportAsCSV();
        public abstract void ApplyChanges();
        public abstract void DeleteItem(TableStruct itemToRemove);

        public class TableStruct : INotifyPropertyChanged
        {
            private long _id;
            private int _rank;
            private DateTime _date;

            public long Id { get { return _id; } set { _id = value; NotifyPropertyChanged("Id"); } }
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

    }
}