using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Rocket_League_Ranking_Tracker.Controller;
using System.Data.SQLite;

namespace Rocket_League_Ranking_Tracker
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class HistoryWindow : Window
    {
        public new string Title { get; set; }
        public ObservableCollection<KeyValuePair<int, int>> LineSeries { get; set; }
        //public ObservableCollection<HistoryWindowControllerBase.TableStruct> Entries { get; set; } 

        readonly HistoryWindowControllerBase _controller;
        string Table { get; set; }
        public HistoryWindow(SQLiteConnection dbConnection, string table)
        {
            InitializeComponent();
            Table = table;
            _controller = new HistoryWindowController(dbConnection, table);

            //Entries = _controller.Entries;
            
            LineSeries = new ObservableCollection<KeyValuePair<int, int>>();

            foreach (HistoryWindowControllerBase.TableStruct tableStruct in _controller.Entries)
            {
                LineSeries.Add(new KeyValuePair<int, int>(tableStruct.ViewId, tableStruct.Rank));
                tableStruct.PropertyChanged += TableEntryChanged;
            }

            //TODO: Fix bindings in xaml to not use datasourcelist as it does now
            var dataSourceList = new List<object>();
            dataSourceList.Add(LineSeries);
            dataSourceList.Add(table);
            LineChart.DataContext = dataSourceList;
            RankHistoryDataGrid.ItemsSource = _controller.Entries;
            _controller.Entries.CollectionChanged += EntriesUpdated;
            Show();
        }

        private void EntriesUpdated(object sender, NotifyCollectionChangedEventArgs e)
        {
            LineSeries.Clear();
            var index = 1;
            foreach (var tableStruct in _controller.Entries)
            {
                tableStruct.ViewId = index;
                index ++;
            }
            foreach (var tableStruct in _controller.Entries)
            {
                LineSeries.Add(new KeyValuePair<int, int>(tableStruct.ViewId, tableStruct.Rank));
                //tableStruct.PropertyChanged += TableEntryChanged;
            }
        }

        private void TableEntryChanged(object sender, PropertyChangedEventArgs e)
        {
            var tableStruct = (HistoryWindowControllerBase.TableStruct)sender;
            foreach (KeyValuePair<int, int> pair in LineSeries.ToList())
            {
                if (pair.Key.Equals(tableStruct.ViewId))
                {
                    LineSeries.Remove(pair);
                    LineSeries.Add(new KeyValuePair<int, int>(tableStruct.ViewId, tableStruct.Rank));
                }
            }
            LineChart.DataContext = LineChart.DataContext;
        }

        private void ExportAsCsvClick(object sender, RoutedEventArgs e)
        {
            _controller.ExportAsCsv();
        }

        private void DeleteItemClick(object sender, RoutedEventArgs e)
        {
            _controller.DeleteItem((HistoryWindowControllerBase.TableStruct)RankHistoryDataGrid.SelectedItem);
        }

    }
}
