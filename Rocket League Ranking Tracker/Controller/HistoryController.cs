using System;
using System.Text;
using System.IO;
using System.Data.SQLite;
using System.Windows;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Windows.Input;

namespace Rocket_League_Ranking_Tracker.Controller
{
    class HistoryWindowController: HistoryWindowControllerBase
    {
        private readonly SQLiteConnection _dbConnection;
        private readonly string _table;

        public HistoryWindowController(SQLiteConnection dbConnection, string table)
        {
            _dbConnection = dbConnection;
            _dbConnection.Update += DatabaseUpdated;
            _table = table;
            DataContext = new ControllerDataContext()
            {
                Entries = new ObservableCollection<TableStruct>(),
                Title = table
            };

            var query = "SELECT * FROM " + table;
            var command = new SQLiteCommand(query, dbConnection);
            var reader = command.ExecuteReader();
            var viewIndex = 1;
            while (reader.Read())
            {
                var entry = new TableStruct() { Id = (long)reader["Id"], ViewIndex = viewIndex, Rank = (int)reader["Rank"], Date = (DateTime)reader["Date"] };
                viewIndex++;
                entry.PropertyChanged += EntryChanged;
                DataContext.Entries.Add(entry);
            }

            DataContext.Entries.CollectionChanged += EntriesUpdated;
        }

        /// <summary>
        /// Exports database to a CSV file
        /// </summary>
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

        /// <summary>
        /// Deletes the selected item in an datagrid
        /// </summary>
        /// <param name="selectedStruct"></param>
        public override void DeleteItem(TableStruct selectedStruct)
        {
            var query = $"DELETE FROM {_table} WHERE Id = {selectedStruct.Id};";
            var command = new SQLiteCommand(query, _dbConnection);
            command.ExecuteNonQuery();
            DataContext.Entries.Remove(selectedStruct);
        }

        /// <summary>
        /// Event listener for when the collection Entries in the DataContext has been updated or changed.
        /// This method updates the viewIndex on all the remaining TableStructs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EntriesUpdated(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action.Equals(NotifyCollectionChangedAction.Add)) return;
            var viewIndex = 1;
            foreach (var tableStruct in DataContext.Entries)
            {
                tableStruct.ViewIndex = viewIndex;
                viewIndex++;
            }
        }

        /// <summary>
        /// Called when the database recieves an update event.
        /// This method inserts newly items into the GUI when new
        /// items are inserted into the database.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                    ViewIndex = DataContext.Entries.Count+1,
                    Rank = (int) reader["Rank"],
                    Date = (DateTime) reader["Date"]
                };
                entry.PropertyChanged += EntryChanged;
                Application.Current.Dispatcher.Invoke(delegate
                {
                    DataContext.Entries.Add(entry);
                });
            }
        }

        /// <summary>
        /// Gets called once an entry in the Datacontext.Entries changes.
        /// This method makes sure that an update to the database happens.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EntryChanged(object sender, PropertyChangedEventArgs e)
        {
            var entry = sender as TableStruct;
            if (entry == null) return;
            if (!EntryExists(entry.Id)) return;
            var query = $"UPDATE {_table} SET Rank = {entry.Rank} , Date = '{entry.Date}' WHERE Id = {entry.Id};";
            var command = new SQLiteCommand(query, _dbConnection);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Checks whether an entry exists or not in the database
        /// </summary>
        /// <param name="id">Database ID to be checked</param>
        /// <returns></returns>
        private bool EntryExists(long id)
        {
            var query = $"SELECT EXISTS(SELECT 1 FROM {_table} WHERE Id = {id} LIMIT 1)";
            var command = new SQLiteCommand(query, _dbConnection);
            var reader = command.ExecuteReader();
            return reader.HasRows;
        }
    }
}
