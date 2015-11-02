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
        public string Title { get; set; }
        public ObservableCollection<KeyValuePair<long, int>> LineSeries { get; set; }

        readonly HistoryWindowControllerBase _controller;
        string Table { get; set; }
        public HistoryWindow(SQLiteConnection dbConnection, string table)
        {
            InitializeComponent();
            Table = table;
            _controller = new HistoryWindowController(dbConnection, table);
            LineSeries = new ObservableCollection<KeyValuePair<long, int>>();

            foreach (HistoryWindowControllerBase.TableStruct tableStruct in _controller.Entries)
            {
                //var tmp = new KeyValuePair<long, int>(tableStruct.Id, tableStruct.Rank);
                LineSeries.Add(new KeyValuePair<long, int>(tableStruct.Id, tableStruct.Rank));
                tableStruct.PropertyChanged += TableEntryChanged;
            }
            //var dataSourceList = new List<List<KeyValuePair<string, int>>>();
            //TODO: Fix bindings in xaml to not use datasourcelist as it does now
            var dataSourceList = new List<object>();
            dataSourceList.Add(LineSeries);
            dataSourceList.Add(table);
            LineChart.DataContext = dataSourceList;
            RankHistoryDataGrid.ItemsSource = _controller.Entries;
            _controller.Entries.CollectionChanged += EntriesUpdated;
            //this.Width = 50 * tmp.Count;
            Show();
        }

        private void EntriesUpdated(object sender, NotifyCollectionChangedEventArgs e)
        {
            LineSeries.Clear();


            foreach (var tableStruct in _controller.Entries)
            {
                LineSeries.Add(new KeyValuePair<long, int>(tableStruct.Id, tableStruct.Rank));
                tableStruct.PropertyChanged += TableEntryChanged;
            }
        }

        private void TableEntryChanged(object sender, PropertyChangedEventArgs e)
        {
            var tableStruct = (HistoryWindowControllerBase.TableStruct)sender;
            foreach (KeyValuePair<long, int> pair in LineSeries.ToList())
            {
                if (pair.Key.Equals(tableStruct.Id))
                {
                    LineSeries.Remove(pair);
                    LineSeries.Add(new KeyValuePair<long, int>(tableStruct.Id, tableStruct.Rank));
                }
            }
            LineChart.DataContext = LineChart.DataContext;
        }

        private void ExportToExcelButonClick(object sender, RoutedEventArgs e)
        {
            _controller.ExportToExcel();
        }

        private void ExportAsCsvClick(object sender, RoutedEventArgs e)
        {
            _controller.ExportAsCSV();
        }

        private void ApplyChangesButtonClick(object sender, RoutedEventArgs e)
        {
            _controller.ApplyChanges();
        }

        private void DeleteItemClick(object sender, RoutedEventArgs e)
        {
            _controller.DeleteItem((HistoryWindowControllerBase.TableStruct)RankHistoryDataGrid.SelectedItem);
        }

    }
}
