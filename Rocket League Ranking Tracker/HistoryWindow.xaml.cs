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
using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MessageBox = System.Windows.MessageBox;

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
            _controller.DeleteItem((HistoryWindowControllerBase.RankTableStruct)RankDataGrid.SelectedItem);
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
            _controller.DeleteItem((HistoryWindowControllerBase.RankTableStruct)RankDataGrid.SelectedItem);
            RankDataGrid.Items.Refresh();
        }


        private void SasveAsPictureClick(object sender, RoutedEventArgs e)
        {
            if (RankChart.Series[0] == null)
                {
                    MessageBox.Show("there is nothing to export");
                }
                else
                {
                    var bounds = VisualTreeHelper.GetDescendantBounds(RankChart);

                    var renderBitmap = new RenderTargetBitmap((int)bounds.Width, (int)bounds.Height, 96, 96, PixelFormats.Pbgra32);

                    var isolatedVisual = new DrawingVisual();
                    using (var drawing = isolatedVisual.RenderOpen())
                    {
                        drawing.DrawRectangle(Brushes.White, null, new Rect(new Point(), bounds.Size)); // Optional Background
                        drawing.DrawRectangle(new VisualBrush(RankChart), null, new Rect(new Point(), bounds.Size));
                    }

                    renderBitmap.Render(isolatedVisual);

                    var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                    {
                        FileName = _controller.DataContext.Title + " " + DateTime.Now.ToString().Replace(":", "-"),
                        DefaultExt = "png"
                    };

                    var result = saveFileDialog.ShowDialog();
                    if (result != true) return;
                    var obrCesta = saveFileDialog.FileName;

                    using (var outStream = new FileStream(obrCesta, FileMode.Create))
                    {
                        var encoder = new PngBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                        encoder.Save(outStream);
                    }
                }
            }
        }
}
