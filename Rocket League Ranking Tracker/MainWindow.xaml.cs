using Rocket_League_Ranking_Tracker.Controller;
using Rocket_League_Ranking_Tracker.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Rocket_League_Ranking_Tracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SQLiteConnection dbConnection;
        public MainWindow()
        {
            InitializeComponent();
            string dbPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\Data\database.db";
            string connectionString = @"Data Source =" + dbPath + ";Version=3;";
            if (!File.Exists(dbPath))
            {
                CreateDatabase(dbPath, connectionString);
            }
            
            dbConnection = new SQLiteConnection(connectionString);
            dbConnection.Open();
            
            RankingModel solo = new SoloRanking(dbConnection);
            RankingModel doubles = new DoublesRanking(dbConnection);
            RankingModel soloStandard = new SoloStandardRanking(dbConnection);
            RankingModel standard = new StandardRanking(dbConnection);
            Score scoreModel = new Score();

            ProcessController pc = new ProcessController();

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

            //rm.RocketLeagueProcess = Process.GetProcessesByName("RocketLeague")[0];
            //rm.updateRanking();
            //;
        }

        private void CreateDatabase(string path,string connectionString)
        {
            SQLiteConnection.CreateFile(path);
            SQLiteConnection dbConnection = new SQLiteConnection(connectionString);
            dbConnection.Open();
            ArrayList rankingTables = new ArrayList (new string [] { "SoloRanking", "DualsRanking", "SoloStandardRanking", "StandardRanking" });
            foreach(string table in rankingTables)
            {
                string query = "create table if not exists " + table + " (Id INTEGER Primary Key, Rank int NOT NULL, Date DateTime NOT NULL); ";
                SQLiteCommand command = new SQLiteCommand(query, dbConnection);
                command.ExecuteNonQuery();
            }
            dbConnection.Close();
        }

        private void SoloRankingHistoryButtonClick(object sender, RoutedEventArgs e)
        {
            HistoryWindow historyWindow = new HistoryWindow(dbConnection, "SoloRanking");
        }

        private void DualsRankingHistoryButtonClick(object sender, RoutedEventArgs e)
        {
            HistoryWindow historyWindow = new HistoryWindow(dbConnection, "DualsRanking");
        }

        private void SoloStandardRankingHistoryButtonClick(object sender, RoutedEventArgs e)
        {
            HistoryWindow historyWindow = new HistoryWindow(dbConnection, "SoloStandardRanking");
        }

        private void StandardRankingHistoryButtonClick(object sender, RoutedEventArgs e)
        {
            HistoryWindow historyWindow = new HistoryWindow(dbConnection, "StandardRanking");
        }
    }
}
