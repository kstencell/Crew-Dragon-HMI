using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace CrewDragonHMI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BackgroundWorker fuelIndicatorBackgroundWorker = new BackgroundWorker();
        BackgroundWorker speedIndicatorBackgroundWorker = new BackgroundWorker();
        public MainWindow()
        {
            InitializeComponent();
            InitializeFuelIndicatorBackgroundWorker();
            InitializeSpeedIndicatorBackgroundWorker();
        }

        private void InitializeFuelIndicatorBackgroundWorker()
        {
            fuelIndicatorBackgroundWorker.WorkerReportsProgress = true;
            fuelIndicatorBackgroundWorker.DoWork += FuelIndicatorBackgroundWorker_DoWork;
            fuelIndicatorBackgroundWorker.ProgressChanged += FuelIndicatorBackgroundWorker_ProgressChanged;
            fuelIndicatorBackgroundWorker.RunWorkerAsync();
        }

        private void FuelIndicatorBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                int fuelLevel = MovementModule.getFuelLevel();
                fuelIndicatorBackgroundWorker.ReportProgress(fuelLevel);
                Thread.Sleep(1000);
            }
        }
        private void FuelIndicatorBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            FuelIndicator.Value = e.ProgressPercentage;
        }

        private void InitializeSpeedIndicatorBackgroundWorker()
        {
            speedIndicatorBackgroundWorker.WorkerReportsProgress = true;
            speedIndicatorBackgroundWorker.DoWork += SpeedIndicatorBackgroundWorker_DoWork;
            speedIndicatorBackgroundWorker.ProgressChanged += SpeedIndicatorBackgroundWorker_ProgressChanged;
            speedIndicatorBackgroundWorker.RunWorkerAsync();
        }

        private void SpeedIndicatorBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                int speed = MovementModule.getSpeed();
                speedIndicatorBackgroundWorker.ReportProgress(speed);
                Thread.Sleep(1000);
            }
        }

        private void SpeedIndicatorBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            SpeedIndicator.Value = e.ProgressPercentage;
        }
    }
}
