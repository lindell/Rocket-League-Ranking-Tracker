using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;

namespace Rocket_League_Ranking_Tracker
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class HistoryWindow : Window
    {
        private ObservableCollection<TableStruct> entries;
        private ArrayList entriesToRemove;
        private ArrayList entriesToUpdate;
        private SQLiteConnection dbConnection;
        private string table;

        public HistoryWindow(SQLiteConnection dbConnection, string table)
        {
            InitializeComponent();
            this.dbConnection = dbConnection;
            this.table = table;
            entries = new ObservableCollection<TableStruct>();
            entriesToRemove = new ArrayList();
            entriesToUpdate = new ArrayList();

            string query = "SELECT * FROM " + table;
            SQLiteCommand command = new SQLiteCommand(query, dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                var entry = new TableStruct() { Id = (long)reader["Id"], Rank = (int)reader["Rank"], Date = (DateTime)reader["Date"] };
                entry.PropertyChanged += EntryChanged;
                entries.Add(entry);
            }
            rankHistoryDataGrid.ItemsSource = entries;
            Show();
        }

        private void EntryChanged(object sender, PropertyChangedEventArgs e)
        {
            entriesToUpdate.Add((TableStruct)sender);
        }

        private void ExportToExcelButonClick(object sender, RoutedEventArgs e)
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

            var dataRange = workSheet.get_Range("B3","B"+row);

            // Set chart range.

            // Set chart properties.
            chart.ChartType = Microsoft.Office.Interop.Excel.XlChartType.xlXYScatterLines;
            chart.ChartWizard(Source: dataRange,
                Title: table,
                CategoryTitle: "Dates",
                ValueTitle: "Rank");
            chart.SeriesCollection(1).XValues = workSheet.Range["C3", "C" + row];
            chart.SeriesCollection(1).Name = table;
            chart.ChartArea.Left = 250;
            chart.ChartArea.Width = 100 + row*40;
            excelApp.Visible = true;

        }

        private void ExportAsCsvClick(object sender, RoutedEventArgs e)
        {
            string csvString = "Id,Ranking,Date\n";

            string query = "SELECT * FROM " + table;
            SQLiteCommand command = new SQLiteCommand(query, dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                var id = (long) reader["Id"];
                var rank = (int) reader["Rank"];
                var date = (DateTime) reader["Date"];
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

        private void ApplyChangesButtonClick(object sender, RoutedEventArgs e)
        {
            foreach(TableStruct entry in entriesToUpdate)
            {
                string query = string.Format("UPDATE {0} SET Rank = {1} , Date = '{2}' WHERE Id = {3};",table, entry.Rank,entry.Date, entry.Id);
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

        private void DeleteItemClick(object sender, RoutedEventArgs e)
        {
            entriesToRemove.Add((TableStruct)rankHistoryDataGrid.SelectedItem);
            entries.Remove((TableStruct)rankHistoryDataGrid.SelectedItem);
            rankHistoryDataGrid.Items.Refresh();
        }


        private class TableStruct : INotifyPropertyChanged
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
