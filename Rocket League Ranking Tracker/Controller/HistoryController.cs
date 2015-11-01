using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using System.Data.SQLite;
using System.Windows;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections;
using Rocket_League_Ranking_Tracker.Model;

namespace Rocket_League_Ranking_Tracker.Controller
{
    class HistoryWindowController: HistoryWindowControllerBase
    {
        private SQLiteConnection dbConnection;
        private string table;

        public HistoryWindowController(SQLiteConnection dbConnection, string table)
        {
            this.dbConnection = dbConnection;
            this.table = table;
            Entries = new ObservableCollection<TableStruct>();
            entriesToRemove = new ArrayList();
            entriesToUpdate = new ArrayList();

            string query = "SELECT * FROM " + table;
            SQLiteCommand command = new SQLiteCommand(query, dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            
            //Fill entry list with entries to be shown 
            while (reader.Read())
            {
                var entry = new TableStruct() { Id = (long)reader["Id"], Rank = (int)reader["Rank"], Date = (DateTime)reader["Date"] };
                entry.PropertyChanged += EntryChanged;
                Entries.Add(entry);
            }
            
        }

        private void EntryChanged(object sender, PropertyChangedEventArgs e)
        {
            entriesToUpdate.Add((TableStruct)sender);
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
                System.Windows.MessageBox.Show("Problem launching Excel. Make sure excel is installed properly and try again.", "Error", new MessageBoxButton(), MessageBoxImage.Error);
                return;
            }

            excelApp.Workbooks.Add();

            Excel._Worksheet workSheet = (Excel.Worksheet)excelApp.ActiveSheet;
            //Initiate appearance of workbook
            var row = 1;
            workSheet.Cells[row, "A"] = table;
            workSheet.Cells[row, "A"].Font.Bold = true;
            row++;
            workSheet.Cells[row, "A"] = "Id";
            workSheet.Cells[row, "B"] = "Rank";
            workSheet.Cells[row, "C"] = "Date";

            // Read from database
            string query = "SELECT * FROM " + table;
            SQLiteCommand command = new SQLiteCommand(query, dbConnection);
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
                Title: table,
                CategoryTitle: "Dates",
                ValueTitle: "Rank");
            chart.SeriesCollection(1).XValues = workSheet.Range["C3", "C" + row];
            chart.SeriesCollection(1).Name = table;
            //((Excel.Axis)chart.Axes(Excel.XlAxisGroup.xlSecondary)).Type = Excel.XlAxisType.xlCategory;
            Excel.Axis axis = (Excel.Axis)chart.Axes(
                    Excel.XlAxisType.xlSeriesAxis,
                    Excel.XlAxisGroup.xlSecondary);
            axis.Type = 
            chart.ChartArea.Left = 250;
            chart.ChartArea.Width = 100 + row * 20;
            excelApp.Visible = true;
        }

        public override void ExportAsCSV()
        {
            string csvString = "Id,Ranking,Date\n";

            string query = "SELECT * FROM " + table;
            SQLiteCommand command = new SQLiteCommand(query, dbConnection);
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
            foreach (TableStruct entry in entriesToUpdate)
            {
                string query = string.Format("UPDATE {0} SET Rank = {1} , Date = '{2}' WHERE Id = {3};", table, entry.Rank, entry.Date, entry.Id);
                SQLiteCommand command = new SQLiteCommand(query, dbConnection);
                command.ExecuteNonQuery();

            }
            entriesToUpdate.Clear();

            foreach (TableStruct entry in entriesToRemove)
            {
                string query = " DELETE FROM " + table + " WHERE Id = " + entry.Id + ";";
                SQLiteCommand command = new SQLiteCommand(query, dbConnection);
                command.ExecuteNonQuery();
            }
            entriesToRemove.Clear();
        }

        public override void DeleteItem(TableStruct itemToRemove)
        {
            entriesToRemove.Add(itemToRemove);
            Entries.Remove(itemToRemove);
        }
    }
}
