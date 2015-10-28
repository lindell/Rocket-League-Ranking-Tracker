using Rocket_League_Ranking_Tracker.Controller;
using Rocket_League_Ranking_Tracker.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public MainWindow()
        {
            InitializeComponent();

            RankingModel solo = new SoloRanking();
            RankingModel duel = new DuelRanking();
            RankingModel standard = new StandardRanking();

            ProcessController pc = new ProcessController();

            pc.AddRankingModel(solo);
            pc.AddRankingModel(duel);
            pc.AddRankingModel(standard);

            soloRanking.DataContext = solo;
            duelRanking.DataContext = duel;
            standardRanking.DataContext = standard;

            //rm.RocketLeagueProcess = Process.GetProcessesByName("RocketLeague")[0];
            //rm.updateRanking();
            //;
        }
    }
}
