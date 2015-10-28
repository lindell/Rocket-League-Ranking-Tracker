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
            RankingModel duals = new DualsRanking();
            RankingModel standard = new StandardRanking();
            Score scoreModel = new Score();

            ProcessController pc = new ProcessController();

            pc.AddMemoryHandler(solo);
            pc.AddMemoryHandler(duals);
            pc.AddMemoryHandler(standard);
            pc.AddMemoryHandler(scoreModel);

            soloRanking.DataContext = solo;
            dualsRanking.DataContext = duals;
            standardRanking.DataContext = standard;
            scores.DataContext = scoreModel;

            //rm.RocketLeagueProcess = Process.GetProcessesByName("RocketLeague")[0];
            //rm.updateRanking();
            //;
        }
    }
}
