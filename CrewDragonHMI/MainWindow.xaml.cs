﻿using System;
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
        // *********************
        // Energy Module Threads
        // *********************
        BackgroundWorker BW_battery = new BackgroundWorker();
        BackgroundWorker BW_generator = new BackgroundWorker();
        BackgroundWorker BW_shields = new BackgroundWorker();

        // *********************
        // Alert Module Threads
        // *********************
        BackgroundWorker BW_alert = new BackgroundWorker();

        // ***********************
        // Movement Module Threads
        // ***********************
        BackgroundWorker BW_fuel = new BackgroundWorker();
        BackgroundWorker BW_warpDrive = new BackgroundWorker();

        // *********************************
        // Exterior Integrity Module Threads
        // *********************************
        BackgroundWorker BW_hull = new BackgroundWorker();
        BackgroundWorker BW_damage = new BackgroundWorker();

        public MainWindow()
        {
            InitializeComponent();
            InitializeEnergyModule();
            InitializeAlertModule();
            InitializeMovementModule();
            InitializeExteriorIntegrityModule();
        }

        /*******************************************/
        /********** ENERGY MODULE METHODS **********/
        /*******************************************/

        private void InitializeEnergyModule()
        {
            BW_battery.WorkerReportsProgress = true;
            BW_battery.DoWork += Battery_DoWork;
            BW_battery.ProgressChanged += Battery_ProgressChanged;
            BW_battery.RunWorkerAsync();

            BW_generator.WorkerReportsProgress = false;
            BW_generator.WorkerSupportsCancellation = true;
            BW_generator.DoWork += Generator_DoWork;

            BW_shields.WorkerReportsProgress = false;
            BW_shields.WorkerSupportsCancellation = true;
            BW_shields.DoWork += Shields_DoWork;

        }

        // ***************
        // *** BATTERY ***
        // ***************

        private void Battery_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                int batteryLevel = EnergyModule.getBatteryLevel();
                BW_battery.ReportProgress(batteryLevel);
                System.Threading.Thread.Sleep(250);
            }
        }

        private void Battery_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            battery.Value = e.ProgressPercentage;
            batteryText.Text = "Battery Level: " + e.ProgressPercentage.ToString() + "%";
        }

        // *****************
        // *** GENERATOR ***
        // *****************

        private void Generator_DoWork(object sender, DoWorkEventArgs e)
        {
            float requestAmount = 0.5f;
            while (!BW_generator.CancellationPending)
            {
                if (EnergyModule.getBatteryLevel() < 100)
                {
                    if (MovementModule.requestFuel(requestAmount))
                    {
                        EnergyModule.generateEnergy();
                    }
                }
                
                if (MovementModule.getFuelLevel() < requestAmount)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        generator.IsChecked = false;
                    });
                }
                System.Threading.Thread.Sleep(1000);
            }
        }

        private void generator_Checked(object sender, RoutedEventArgs e)
        {
            if (MovementModule.getFuelLevel() < 0.5F)
            {
                this.Dispatcher.Invoke(() =>
                {
                    generator.IsChecked = false;
                });
                return;
            }
            if (!BW_generator.IsBusy)
            {
                EnergyModule.toggleGeneratorStatus();
                BW_generator.RunWorkerAsync();
            }
        }

        private void generator_Unchecked(object sender, RoutedEventArgs e)
        {
            if (BW_generator.IsBusy)
            {
                EnergyModule.toggleGeneratorStatus();
                BW_generator.CancelAsync();
            }
        }

        // *****************
        // **** SHIELDS ****
        // *****************

        private void Shields_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!BW_shields.CancellationPending)
            {
                if (!EnergyModule.requestEnergy(1.0F))
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        shields.IsChecked = false;
                    });
                }
                System.Threading.Thread.Sleep(1000);
            }
        }

        private void shields_Checked(object sender, RoutedEventArgs e)
        {
            if (EnergyModule.getBatteryLevel() < 1.0F)
            {
                this.Dispatcher.Invoke(() =>
                {
                    shields.IsChecked = false;
                });
                return;
            }
            if (!BW_shields.IsBusy)
            {
                EnergyModule.toggleShieldStatus();
                BW_shields.RunWorkerAsync();
            }

        }

        private void shields_Unchecked(object sender, RoutedEventArgs e)
        {
            if (BW_shields.IsBusy)
            {
                EnergyModule.toggleShieldStatus();
                BW_shields.CancelAsync();
            }
        }


        /*******************************************/
        /********** ALERT MODULE METHODS ***********/
        /*******************************************/

        private void InitializeAlertModule()
        {
            // not sure if this needs a thread
        }


        /*******************************************/
        /********* MOVEMENT MODULE METHODS *********/
        /*******************************************/
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
            fuel.Value = e.ProgressPercentage;
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

                if (MovementModule.requestFuel(0.2f))
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        speedSlider.Value = speedSlider.Maximum;
                        speedText.Text = "Speed: LIGHT SPEED";
                    });
                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        warpDrive.IsChecked = false;
                    });
                    
                }
                System.Threading.Thread.Sleep(200);
            }
        }

        private void warpDrive_Checked(object sender, RoutedEventArgs e)
        {
            if (MovementModule.getFuelLevel() < 0.2f)
            {
                this.Dispatcher.Invoke(() =>
                {
                    warpDrive.IsChecked = false;
                    speedSlider.Value = MovementModule.getSpeed();
                    speedText.Text = "Speed: " + (int)speedSlider.Value + " KM/S";
                });
                return;
            }
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
        /*****************************************************/
        /********* EXTERIOR INTEGRITY MODULE METHODS *********/
        /*****************************************************/
        private void InitializeExteriorIntegrityModule()
        {
            BW_hull.WorkerReportsProgress = true;
            BW_hull.DoWork += Hull_DoWork;
            BW_hull.ProgressChanged += Hull_ProgressChanged;
            BW_hull.RunWorkerAsync();

            BW_damage.WorkerReportsProgress = false;
            BW_damage.DoWork += Damage_DoWork;
            BW_damage.RunWorkerAsync();
        }

        // ****************
        // ***** HULL *****
        // ****************

        private void Hull_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                float hullIntegrity = ExteriorIntegrityModule.getHullIntegrity();
                BW_hull.ReportProgress((int)hullIntegrity); // I'm casting this to an int as HullIntegrity is a float. We should probably agree on just using ints or floats
                System.Threading.Thread.Sleep(250);
            }
        }
        private void Hull_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            hullText.Text = "Hull Integrity: " + e.ProgressPercentage.ToString() + "%";
        }

        // ******************
        // ***** DAMAGE *****
        // ******************

        private void Damage_DoWork(object sender, DoWorkEventArgs e)
        {
            while (ExteriorIntegrityModule.getHullIntegrity() > 0)
            {
                if (MovementModule.getWarpDriveStatus())
                {
                    ExteriorIntegrityModule.takeDamage(2);
                }
                else
                {
                    float damage = ((float)MovementModule.getSpeed()) / 1000;
                    ExteriorIntegrityModule.takeDamage(damage);
                }
                System.Threading.Thread.Sleep(1000);
            }

            Environment.Exit(0);
        }
    }
}
