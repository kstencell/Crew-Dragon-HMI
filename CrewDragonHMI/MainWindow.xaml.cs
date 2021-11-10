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

        BackgroundWorker batteryIndicatorBackgroundWorker = new BackgroundWorker();
        BackgroundWorker generatorIndicatorBackgroundWorker = new BackgroundWorker();
        public MainWindow()
        {
            InitializeComponent();
            InitializeBatteryIndicatorBackgroundWorker();
            InitializeGeneratorIndicatorBackgroundWorker();
        }

        private void InitializeBatteryIndicatorBackgroundWorker()
        {
            batteryIndicatorBackgroundWorker.WorkerReportsProgress = true;
            batteryIndicatorBackgroundWorker.DoWork += BatteryIndicatorBackgroundWorker_DoWork;
            batteryIndicatorBackgroundWorker.ProgressChanged += BatteryIndicatorBackgroundWorker_ProgressChanged;
            batteryIndicatorBackgroundWorker.RunWorkerAsync();
        }

        private void BatteryIndicatorBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                int batteryLevel = EnergyModule.getBatteryLevel();
                batteryIndicatorBackgroundWorker.ReportProgress(batteryLevel);
                System.Threading.Thread.Sleep(1000);
            }
        }

        private void BatteryIndicatorBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            BatteryIndicator.Value = e.ProgressPercentage;
            BatteryIndicatorLabel.Text = "Battery Level: " + e.ProgressPercentage.ToString() + "%";
        }

        private void BatteryIndicator_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void InitializeGeneratorIndicatorBackgroundWorker()
        {
            generatorIndicatorBackgroundWorker.WorkerReportsProgress = false;
            generatorIndicatorBackgroundWorker.WorkerSupportsCancellation = true;
            generatorIndicatorBackgroundWorker.DoWork += generatorIndicatorBackgroundWorker_DoWork;
        }

        private void generatorIndicatorBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!generatorIndicatorBackgroundWorker.CancellationPending)
            {
                System.Threading.Thread.Sleep(500);
                EnergyModule.setBatteryLevel(EnergyModule.batteryLevel + 2);
            }
        }

        private void GeneratorPower_Checked(object sender, RoutedEventArgs e)
        {

            if (!generatorIndicatorBackgroundWorker.IsBusy)
            {
                generatorIndicatorBackgroundWorker.RunWorkerAsync();
            }
        }

        private void GeneratorPower_Unchecked(object sender, RoutedEventArgs e)
        {
            if (generatorIndicatorBackgroundWorker.IsBusy)
            {
                generatorIndicatorBackgroundWorker.CancelAsync();
            }
        }

        private void BatteryIndicatorLabel_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
