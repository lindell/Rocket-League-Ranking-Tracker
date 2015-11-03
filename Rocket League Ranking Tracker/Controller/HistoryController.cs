using System;
using System.Text;
using System.IO;
using System.Data.SQLite;
using System.Windows;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Common;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Forms.DataVisualization.Charting;
using Microsoft.Windows.Controls;
using Chart = System.Windows.Controls.DataVisualization.Charting.Chart;
using DataGrid = System.Windows.Controls.DataGrid;

namespace Rocket_League_Ranking_Tracker.Controller
{
    class HistoryWindowController: HistoryWindowControllerBase
    {
        private readonly SQLiteConnection _dbConnection;
        private readonly string _table;
        private Chart _lineChart;
        private DataGrid _datagrid;
        private LineChartDataContext LineChartContext;


        public HistoryWindowController(SQLiteConnection dbConnection, string table, Chart lineChart, DataGrid dataGrid)
        {
            _dbConnection = dbConnection;
            _dbConnection.Update += DatabaseUpdated;
            _table = table;
            _lineChart = lineChart;
            _datagrid = dataGrid;
            Entries = new ObservableCollection<TableStruct>();
            EntriesToRemove = new ArrayList();
            EntriesToUpdate = new ArrayList();

            var query = "SELECT * FROM " + table;
            var command = new SQLiteCommand(query, dbConnection);
            var reader = command.ExecuteReader();
            var index = 1;
            //Fill entry list with entries to be shown 

            LineChartContext = new LineChartDataContext()
            {
                LineSeriesData = new ObservableCollection<KeyValuePair<int, int>>(),
                Title = table
            };
            while (reader.Read())
            {
                var entry = new TableStruct() { Id = (long)reader["Id"], ViewId = index, Rank = (int)reader["Rank"], Date = (DateTime)reader["Date"] };
                index++;
                entry.PropertyChanged += EntryChanged;
                Entries.Add(entry);
                LineChartContext.LineSeriesData.Add(new KeyValuePair<int, int>(entry.ViewId, entry.Rank));
                entry.PropertyChanged += TableEntryChanged;

            }

            Entries.CollectionChanged += EntriesUpdated;
            _lineChart.DataContext = LineChartContext;
            _datagrid.ItemsSource = Entries;
            _datagrid.SelectionChanged += TableSelectionChanged;
        }

        private void TableSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //((LineSeries)_lineChart.Series[0]).SelectedItem =
            //    LineChartContext.LineSeriesData[((HistoryWindowControllerBase.TableStruct)_datagrid.SelectedItem).ViewId - 1];
        }


        private void EntriesUpdated(object sender, NotifyCollectionChangedEventArgs e)
        {
            LineChartContext.LineSeriesData.Clear();
            var index = 1;
            foreach (var tableStruct in Entries)
            {
                tableStruct.ViewId = index;
                index++;

                LineChartContext.LineSeriesData.Add(new KeyValuePair<int, int>(tableStruct.ViewId, tableStruct.Rank));
                tableStruct.PropertyChanged += TableEntryChanged;
            }

            _datagrid.Items.Refresh();
        }

        private void TableEntryChanged(object sender, PropertyChangedEventArgs e)
        {
            //var tableStruct = (HistoryWindowControllerBase.TableStruct)sender;
            //foreach (KeyValuePair<int, int> pair in LineChartContext.LineSeriesData.ToList())
            //{
            //    if (pair.Key.Equals(tableStruct.ViewId))
            //    {
            //        LineChartContext.LineSeriesData.Remove(pair);
            //        LineChartContext.LineSeriesData.Add(new KeyValuePair<int, int>(tableStruct.ViewId, tableStruct.Rank));
            //    }
            //}
            //_lineChart.DataContext = _lineChart.DataContext;
        }


        private void DatabaseUpdated(object sender, UpdateEventArgs e)
        {
            if (!e.Event.Equals(UpdateEventType.Insert)) return;
            var query = $"SELECT * FROM {_table} WHERE Id={(int) e.RowId}";
            var command = new SQLiteCommand(query, _dbConnection);
            var reader = command.ExecuteReader();

            //Fill entry list with entries to be shown 
            while (reader.Read())
            {
                var entry = new TableStruct()
                {
                    Id = (long) reader["Id"],
                    ViewId = Entries.Count+1,
                    Rank = (int) reader["Rank"],
                    Date = (DateTime) reader["Date"]
                };
                entry.PropertyChanged += EntryChanged;
                Application.Current.Dispatcher.Invoke(delegate
                {
                    Entries.Add(entry);
                });
            }
        }


        private void EntryChanged(object sender, PropertyChangedEventArgs e)
        {
            var entry = sender as TableStruct;
            if (entry == null) return;
            if (EntryExists(entry.Id))
            {
                var query = $"UPDATE {_table} SET Rank = {entry.Rank} , Date = '{entry.Date}' WHERE Id = {entry.Id};";
                var command = new SQLiteCommand(query, _dbConnection);
                command.ExecuteNonQuery();
            }
        }

        private bool EntryExists(long id)
        {
            var query = $"SELECT EXISTS(SELECT 1 FROM {_table} WHERE Id = {id} LIMIT 1)";
            var command = new SQLiteCommand(query, _dbConnection);
            var reader = command.ExecuteReader();
            return reader.HasRows;
        }


        public override void ExportAsCsv()
        {
            var csvString = "Id,Ranking,Date\n";

            var query = "SELECT * FROM " + _table;
            var command = new SQLiteCommand(query, _dbConnection);
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var id = (long)reader["Id"];
                var rank = (int)reader["Rank"];
                var date = (DateTime)reader["Date"];
                csvString += id + "," + rank + "," + date + "\n";
            }

            //Save File
            var csvFileDialog = new SaveFileDialog
            {
                Filter = "CSV file|*.csv",
                Title = "Save CSV File"
            };
            csvFileDialog.ShowDialog();

            if (csvFileDialog.FileName != "")
            {
                //Create Unicode converter
                UnicodeEncoding uniEncoding = new UnicodeEncoding();

                //Get Filestream
                try
                {
                    FileStream fs = (FileStream)csvFileDialog.OpenFile();

                    //Write to file
                    fs.Write(uniEncoding.GetBytes(csvString), 0, uniEncoding.GetByteCount(csvString));

                    fs.Close();
                }
                catch (IOException)
                {
                    MessageBox.Show("The program can't write to the file!\nPlease close any other program that has the file open.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public override void DeleteItem(TableStruct itemToRemove)
        {
            var query = $"DELETE FROM {_table} WHERE Id = {itemToRemove.Id};";
            var command = new SQLiteCommand(query, _dbConnection);
            command.ExecuteNonQuery();
            Entries.Remove(itemToRemove);
            _datagrid.Items.Refresh();

        }

        private class LineChartDataContext
        {
            public ObservableCollection<KeyValuePair<int, int>> LineSeriesData { get; set; }
            public string Title { get; set; }
        }
    }
}
