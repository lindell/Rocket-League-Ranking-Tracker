using System;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using System.Data.SQLite;
using System.Windows;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections;

namespace Rocket_League_Ranking_Tracker.Controller
{
    class HistoryWindowController: HistoryWindowControllerBase
    {
        private readonly SQLiteConnection _dbConnection;
        private readonly string _table;

        public HistoryWindowController(SQLiteConnection dbConnection, string table)
        {
            _dbConnection = dbConnection;
            dbConnection.Update += DatabaseUpdated;
            _table = table;
            Entries = new ObservableCollection<TableStruct>();
            EntriesToRemove = new ArrayList();
            EntriesToUpdate = new ArrayList();

            var query = "SELECT * FROM " + table;
            var command = new SQLiteCommand(query, dbConnection);
            var reader = command.ExecuteReader();
            
            //Fill entry list with entries to be shown 
            while (reader.Read())
            {
                var entry = new TableStruct() { Id = (long)reader["Id"], Rank = (int)reader["Rank"], Date = (DateTime)reader["Date"] };
                entry.PropertyChanged += EntryChanged;
                Entries.Add(entry);
            }  
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
            EntriesToUpdate.Add((TableStruct)sender);
        }

        public override void ExportToExcel()
        {
            //Start excel app
            Excel.Application excelApp;
            try
            {
                excelApp = new Excel.Application();
            }
            catch (Exception)
            {
                MessageBox.Show("Problem launching Excel. Make sure excel is installed properly and try again.", "Error", new MessageBoxButton(), MessageBoxImage.Error);
                return;
            }

            excelApp.Workbooks.Add();

            Excel._Worksheet workSheet = (Excel.Worksheet)excelApp.ActiveSheet;
            //Initiate appearance of workbook
            var row = 1;
            workSheet.Cells[row, "A"] = _table;
            workSheet.Cells[row, "A"].Font.Bold = true;
            row++;
            workSheet.Cells[row, "A"] = "Id";
            workSheet.Cells[row, "B"] = "Rank";
            workSheet.Cells[row, "C"] = "Date";

            // Read from database
            string query = "SELECT * FROM " + _table;
            SQLiteCommand command = new SQLiteCommand(query, _dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                //Insert into excel workbook
                row++;
                var entry = new TableStruct() { Id = (long)reader["Id"], Rank = (int)reader["Rank"], Date = (DateTime)reader["Date"] };
                workSheet.Cells[row, "A"] = entry.Id;
                workSheet.Cells[row, "B"] = entry.Rank;
                workSheet.Cells[row, "C"] = entry.Date;
            }
            workSheet.Columns.AutoFit();

            // Add chart.
            var charts = workSheet.ChartObjects() as
                Microsoft.Office.Interop.Excel.ChartObjects;
            var chartObject = charts.Add(250, 10, 300, 300) as
                Microsoft.Office.Interop.Excel.ChartObject;
            var chart = chartObject.Chart;

            var dataRange = workSheet.get_Range("B3", "B" + row);

            // Set chart range.

            // Set chart properties.
            chart.ChartType = Microsoft.Office.Interop.Excel.XlChartType.xlLineMarkers;
            chart.ChartWizard(Source: dataRange,
                Title: _table,
                CategoryTitle: "Dates",
                ValueTitle: "Rank");
            chart.SeriesCollection(1).XValues = workSheet.Range["C3", "C" + row];
            chart.SeriesCollection(1).Name = _table;
            //((Excel.Axis)chart.Axes(Excel.XlAxisGroup.xlSecondary)).Type = Excel.XlAxisType.xlCategory;
            Excel.Axis axis = (Excel.Axis)chart.Axes(
                    Excel.XlAxisType.xlSeriesAxis,
                    Excel.XlAxisGroup.xlSecondary);
            chart.ChartArea.Left = 250;
            chart.ChartArea.Width = 100 + row * 20;
            excelApp.Visible = true;
        }

        public override void ExportAsCsv()
        {
            string csvString = "Id,Ranking,Date\n";

            string query = "SELECT * FROM " + _table;
            SQLiteCommand command = new SQLiteCommand(query, _dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();

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
                    FileStream fs = (System.IO.FileStream)csvFileDialog.OpenFile();

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

        public override void ApplyChanges()
        {
            foreach (TableStruct entry in EntriesToUpdate)
            {
                string query = string.Format("UPDATE {0} SET Rank = {1} , Date = '{2}' WHERE Id = {3};", _table, entry.Rank, entry.Date, entry.Id);
                SQLiteCommand command = new SQLiteCommand(query, _dbConnection);
                command.ExecuteNonQuery();

            }
            EntriesToUpdate.Clear();

            foreach (TableStruct entry in EntriesToRemove)
            {
                string query = " DELETE FROM " + _table + " WHERE Id = " + entry.Id + ";";
                SQLiteCommand command = new SQLiteCommand(query, _dbConnection);
                command.ExecuteNonQuery();
            }
            EntriesToRemove.Clear();
        }

        public override void DeleteItem(TableStruct itemToRemove)
        {
            EntriesToRemove.Add(itemToRemove);
            Entries.Remove(itemToRemove);
        }
    }
}
