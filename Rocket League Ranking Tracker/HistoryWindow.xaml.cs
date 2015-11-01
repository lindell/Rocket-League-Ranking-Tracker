using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
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
using Rocket_League_Ranking_Tracker.Controller;
using System.Data.SQLite;
using Rocket_League_Ranking_Tracker.Model;
using Charting = System.Windows.Controls.DataVisualization.Charting;

namespace Rocket_League_Ranking_Tracker
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class HistoryWindow : Window
    {

        HistoryWindowControllerBase controller;
        string Table { get; set; }
        public HistoryWindow(SQLiteConnection dbConnection, string table)
        {
            InitializeComponent();
            this.Table = table;
            controller = new HistoryWindowController(dbConnection, table);
            List<KeyValuePair<string, int>> tmp = new List<KeyValuePair<string, int>>();

            foreach (HistoryWindowControllerBase.TableStruct tableStruct in controller.Entries)
            {
                tmp.Add(new KeyValuePair<string, int>(tableStruct.Date.ToString(), tableStruct.Rank));
            }
            //var dataSourceList = new List<List<KeyValuePair<string, int>>>();
            //TODO: Fix bindings in xaml to not use datasourcelist as it does now
            var dataSourceList = new List<object>();
            dataSourceList.Add(tmp);
            dataSourceList.Add(table);
            lineChart.DataContext = dataSourceList;
            rankHistoryDataGrid.ItemsSource = controller.Entries;
            //this.Width = 50 * tmp.Count;
            Show();

        }

        private void ExportToExcelButonClick(object sender, RoutedEventArgs e)
        {
            controller.ExportToExcel();
        }

        private void ExportAsCsvClick(object sender, RoutedEventArgs e)
        {
            controller.ExportAsCSV();
        }

        private void ApplyChangesButtonClick(object sender, RoutedEventArgs e)
        {
            controller.ApplyChanges();
        }

        private void DeleteItemClick(object sender, RoutedEventArgs e)
        {
            controller.DeleteItem((HistoryWindowControllerBase.TableStruct)rankHistoryDataGrid.SelectedItem);
        }

    }
}
