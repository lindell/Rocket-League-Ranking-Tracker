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
using DataGrid = Microsoft.Windows.Controls.DataGrid;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

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
            _controller = new HistoryWindowController(dbConnection, table);
            DataContext = _controller.DataContext;
            Show();
        }

        private void ExportAsCsvClick(object sender, RoutedEventArgs e)
        {
            _controller.ExportAsCsv();
        }

        private void DeleteItemClick(object sender, RoutedEventArgs e)
        {
            _controller.DeleteItem((HistoryWindowControllerBase.TableStruct)RankDataGrid.SelectedItem);
            RankDataGrid.Items.Refresh();
        }

        /// <summary>
        /// Listens to delete key strokes and calls DeleteItem.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DataGridPreviewKeyDown(object sender, KeyEventArgs e)
        { 
            if (e.Key != Key.Delete) return;
            _controller.DeleteItem((HistoryWindowControllerBase.TableStruct)RankDataGrid.SelectedItem);
            RankDataGrid.Items.Refresh();
        }
    }
}
