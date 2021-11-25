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
using System.Windows.Controls.Primitives;

namespace CrewDragonHMI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BackgroundWorker BW_fuel = new BackgroundWorker();
        BackgroundWorker BW_speed = new BackgroundWorker();
        BackgroundWorker BW_direction = new BackgroundWorker();
        BackgroundWorker BW_warpDrive = new BackgroundWorker();

        public MainWindow()
        {
            InitializeComponent();
            InitializeMovementModule();
        }

        private void InitializeMovementModule()
        {
            BW_fuel.WorkerReportsProgress = true;
            BW_fuel.DoWork += Fuel_DoWork;
            BW_fuel.ProgressChanged += Fuel_ProgressChanged;
            BW_fuel.RunWorkerAsync();

            BW_warpDrive.WorkerReportsProgress = false;
            BW_warpDrive.WorkerSupportsCancellation = true;
            BW_warpDrive.DoWork += WarpDrive_DoWork;
        }

        //******************************
        //********** FUEL **************
        //******************************
        private void Fuel_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                float fuelLevel = MovementModule.getFuelLevel();
                BW_fuel.ReportProgress((int)fuelLevel);
                Thread.Sleep(250);
            }
        }
        private void Fuel_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            FuelIndicator.Value = e.ProgressPercentage;
            fuelText.Text = "Fuel: " + e.ProgressPercentage.ToString() + "%";
        }


        //******************************
        //********** SPEED *************
        //******************************
        private void SpeedSlider_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (!MovementModule.getWarpDriveStatus())
            {
                if (MovementModule.requestSpeedChange((int)speedSlider.Value))
                {
                    speedText.Text = "Speed: " + (int)speedSlider.Value + " KM/S";
                }
                else
                {
                    speedSlider.Value = MovementModule.getSpeed();
                    speedText.Text = "Speed: " + (int)speedSlider.Value + " KM/S";
                }
            }
            else
            {
                speedSlider.Value = MovementModule.getSpeed();
                speedText.Text = "Speed: " + (int)speedSlider.Value + " KM/S";
            }
        }

        //******************************
        //******** DIRECTION ***********
        //******************************
        private void DirectionSlider_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (!MovementModule.getWarpDriveStatus())
            {
                if (MovementModule.requestDirectionChange((int)directionSlider.Value))
                {
                    directionText.Text = "Direction: " + (int)directionSlider.Value + " Degrees";
                }
                else
                {
                    directionSlider.Value = MovementModule.getDirection();
                    directionText.Text = "Direction: " + (int)directionSlider.Value + " Degrees";
                }
            }
            else
            {
                directionSlider.Value = MovementModule.getDirection();
                directionText.Text = "Direction: " + (int)directionSlider.Value + " Degrees";
            }
        }

        //******************************
        //********* WARP DRIVE *********
        //******************************

        private void WarpDrive_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!BW_warpDrive.CancellationPending)
            {
                int previousSpeed = MovementModule.getSpeed();

                if (MovementModule.requestFuel(1))
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        speedSlider.Value = speedSlider.Maximum;
                        speedText.Text = "Speed: LIGHT SPEED";
                    });
                    System.Threading.Thread.Sleep(1000);
                }
                
            }
        }

        private void warpDrive_Checked(object sender, RoutedEventArgs e)
        {
            if (!BW_warpDrive.IsBusy)
            {
                MovementModule.toggleWarpDrive();
                BW_warpDrive.RunWorkerAsync();
            }
        }

        private void warpDrive_Unchecked(object sender, RoutedEventArgs e)
        {
            if (BW_warpDrive.IsBusy)
            {
                MovementModule.toggleWarpDrive();
                BW_warpDrive.CancelAsync();
                this.Dispatcher.Invoke(() =>
                {
                    speedSlider.Value = MovementModule.getSpeed();
                    speedText.Text = "Speed: " + (int)speedSlider.Value + " KM/S";
                });
            }
        }
    }
}
