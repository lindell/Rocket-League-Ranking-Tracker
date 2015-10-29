using System;
using System.Collections;
using System.Collections.Generic;
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
        public HistoryWindow(SQLiteConnection dbConnection, string table)
        {
            InitializeComponent();
            string query = "SELECT * FROM " + table;
            SQLiteCommand command = new SQLiteCommand(query, dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            ArrayList entries = new ArrayList();
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
            throw new NotImplementedException();
        }
    }

    public class TableStruct
    {
        public long Id { get; set; }
        public int Rank { get; set; }
        public DateTime Date { get; set; }
    }
}
