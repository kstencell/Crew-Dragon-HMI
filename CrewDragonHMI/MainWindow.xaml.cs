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
        public MainWindow()
        {
            InitializeComponent();
            InitializeFuelIndicatorBackgroundWorker();
        }

        private void InitializeFuelIndicatorBackgroundWorker()
        {
            fuelIndicatorBackgroundWorker.WorkerReportsProgress = true;
            fuelIndicatorBackgroundWorker.DoWork += FuelBackgroundWorker_DoWork;
            fuelIndicatorBackgroundWorker.ProgressChanged += FuelBackgroundWorker_ProgressChanged;
            fuelIndicatorBackgroundWorker.RunWorkerAsync();
        }

        private void FuelBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                int fuelLevel = MovementModule.getFuelLevel();
                fuelIndicatorBackgroundWorker.ReportProgress(fuelLevel);
                System.Threading.Thread.Sleep(1000);
            }
        }
        private void FuelBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            FuelIndicator.Value = e.ProgressPercentage;
        }

        private void Fuel_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }
    }
}
