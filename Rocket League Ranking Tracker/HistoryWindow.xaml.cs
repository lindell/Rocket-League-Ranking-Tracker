using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Rocket_League_Ranking_Tracker.Controller;
using System.Data.SQLite;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Effects;

namespace Rocket_League_Ranking_Tracker
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class HistoryWindow : Window
    {
        public new string Title { get; set; }

        readonly HistoryWindowControllerBase _controller;

        public HistoryWindow(SQLiteConnection dbConnection, string table)
        {
            InitializeComponent();
            _controller = new HistoryWindowController(dbConnection, table, LineChart, RankDataGrid);
            Show();
        }

        private void ExportAsCsvClick(object sender, RoutedEventArgs e)
        {
            _controller.ExportAsCsv();
        }

        private void DeleteItemClick(object sender, RoutedEventArgs e)
        {
            _controller.DeleteItem((HistoryWindowControllerBase.TableStruct)RankDataGrid.SelectedItem);
        }

    }
}
