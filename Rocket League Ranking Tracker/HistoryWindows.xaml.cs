using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SQLite;
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

namespace Rocket_League_Ranking_Tracker
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class HistoryWindow : Window
    {
        private ArrayList entries;
        private ArrayList entriesToRemove;
        private SQLiteConnection dbConnection;
        private string table;

        public HistoryWindow(SQLiteConnection dbConnection, string table)
        {
            InitializeComponent();
            this.dbConnection = dbConnection;
            this.table = table;
            entries = new ArrayList();
            entriesToRemove = new ArrayList();

            string query = "SELECT * FROM " + table;
            SQLiteCommand command = new SQLiteCommand(query, dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                entries.Add(new TableStruct() { Id = (long)reader["Id"], Rank = (int)reader["Rank"], Date = (DateTime)reader["Date"] });
            }
            lvRankHistory.ItemsSource = entries;
            Show();
        }

        private void ExportToExcelButonClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ApplyChangesButtonClick(object sender, RoutedEventArgs e)
        {
            foreach(TableStruct entry in entriesToRemove)
            {
                string query = " DELETE FROM " + table + " WHERE Id = " + entry.Id + ";";
                SQLiteCommand command = new SQLiteCommand(query, dbConnection);
                command.ExecuteNonQuery();
            }
        }

        private void EditItemClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void DeleteItemClick(object sender, RoutedEventArgs e)
        {
            entriesToRemove.Add((TableStruct)lvRankHistory.SelectedItem);
            entries.Remove((TableStruct)lvRankHistory.SelectedItem);
            lvRankHistory.Items.Refresh();
        }

        private class TableStruct
        {
            public long Id { get; set; }
            public int Rank { get; set; }
            public DateTime Date { get; set; }
        }
    }


}
