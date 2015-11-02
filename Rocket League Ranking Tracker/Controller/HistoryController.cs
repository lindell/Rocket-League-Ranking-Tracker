using System;
using System.Text;
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
