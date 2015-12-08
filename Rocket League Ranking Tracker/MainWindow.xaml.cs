using Rocket_League_Ranking_Tracker.Controller;
using Rocket_League_Ranking_Tracker.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;

namespace Rocket_League_Ranking_Tracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly SQLiteConnection _dbConnection;
        private readonly NotifyIcon _notifyIcon;
        public MainWindow()
        {
            InitializeComponent();
            string dbPath = "database.db";
            string connectionString = @"Data Source =" + dbPath + ";Version=3;";

            CreateDatabase(dbPath, connectionString);
            
            _dbConnection = new SQLiteConnection(connectionString);
            _dbConnection.Open();

            RankingModel solo = new SoloRanking(_dbConnection);
            RankingModel doubles = new DoublesRanking(_dbConnection);
            RankingModel soloStandard = new SoloStandardRanking(_dbConnection);
            RankingModel standard = new StandardRanking(_dbConnection);

            var scoreModel = new Score();

            var pc = new ProcessController();

            pc.AddMemoryHandler(solo);
            pc.AddMemoryHandler(doubles);
            pc.AddMemoryHandler(soloStandard);
            pc.AddMemoryHandler(standard);
            pc.AddMemoryHandler(scoreModel);

            soloRanking.DataContext = solo;
            doublesRanking.DataContext = doubles;
            soloStandardRanking.DataContext = soloStandard;
            standardRanking.DataContext = standard;
            scores.DataContext = scoreModel;
            processInfo.DataContext = pc;

            _notifyIcon = new NotifyIcon();
            _notifyIcon.Icon = new System.Drawing.Icon("logo.ico");
            _notifyIcon.Visible = true;
            _notifyIcon.Click +=
                delegate (object sender, EventArgs args)
                {
                    this.Show();
                    this.WindowState = WindowState.Normal;
                };
            Closing += OnWindowClosing;
        }

        private void CreateDatabase(string path,string connectionString)
        {
            if (!File.Exists(path))
            {
                SQLiteConnection.CreateFile(path);
            }
            var dbConnection = new SQLiteConnection(connectionString);
            dbConnection.Open();
            var rankingTables = new ArrayList (new string [] { "SoloRanking", "DualsRanking", "SoloStandardRanking", "StandardRanking" });
            var scoreTables = new ArrayList(new string[] {"SoloGoals", "DualsGoals", "SoloStandardGoals", "StandardGoals"});
            var columns = new List<string> { "Rank", "Date", "OrangeGoals", "BlueGoals" };

            //TODO: Clean up, now is a mess
            foreach (string tableName in rankingTables)
            {
                var query = "create table if not exists " + tableName + " (Id INTEGER Primary Key, Rank int NOT NULL, Date DateTime NOT NULL, OrangeGoals int, BlueGoals int); ";
                var command = new SQLiteCommand(query, dbConnection);
                command.ExecuteNonQuery();

                using (var con = new SQLiteConnection(connectionString))
                {
                    using (var cmd = new SQLiteCommand("PRAGMA table_info(" + tableName + ");"))
                    {
                        var table = new DataTable();

                        cmd.Connection = con;
                        cmd.Connection.Open();

                        SQLiteDataAdapter adp = null;
                        try
                        {
                            adp = new SQLiteDataAdapter(cmd);
                            adp.Fill(table);
                            con.Close();
                            foreach (var column in columns)
                            {
                                bool match = false;
                                var list = table.AsEnumerable().Select(r => r["name"].ToString()).ToList();
                                foreach (var tableColumn in list)
                                {
                                    if (column == tableColumn)
                                        match = true;

                                }
                                if (!match)
                                {
                                    query = $"ALTER TABLE {tableName} ADD {column} int;";
                                    command = new SQLiteCommand(query, dbConnection);
                                    command.ExecuteNonQuery();
                                }
                            }
                        }
                        catch (Exception ex)
                        { }
                    }
                }
            }
            foreach (string tableName in scoreTables)
            {
                var query = "create table if not exists " + tableName + " (Id INTEGER Primary Key, GameId Int, Time Int, Team Text); ";
                var command = new SQLiteCommand(query, dbConnection);
                command.ExecuteNonQuery();
            }
            dbConnection.Close();
        }

        private void SoloRankingHistoryButtonClick(object sender, RoutedEventArgs e)
        {
            new HistoryWindow(_dbConnection, "SoloRanking") {Owner = this};
        }

        private void DualsRankingHistoryButtonClick(object sender, RoutedEventArgs e)
        {
            new HistoryWindow(_dbConnection, "DualsRanking") { Owner = this };
        }

        private void SoloStandardRankingHistoryButtonClick(object sender, RoutedEventArgs e)
        {
            new HistoryWindow(_dbConnection, "SoloStandardRanking") { Owner = this };
        }

        private void StandardRankingHistoryButtonClick(object sender, RoutedEventArgs e)
        {
            new HistoryWindow(_dbConnection, "StandardRanking") { Owner = this };
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
                Hide();

            base.OnStateChanged(e);
        }

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            _notifyIcon.Visible = false;
        }
    }
}
