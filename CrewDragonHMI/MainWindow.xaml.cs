﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        BackgroundWorker BW_speed = new BackgroundWorker();
        BackgroundWorker BW_rotation = new BackgroundWorker();

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
                System.Threading.Thread.Sleep(1000);
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
            while (!BW_generator.CancellationPending)
            {
                EnergyModule.setBatteryLevel(EnergyModule.batteryLevel + 1);
                System.Threading.Thread.Sleep(250);
            }
        }

        private void generator_Checked(object sender, RoutedEventArgs e)
        {
            if (!BW_generator.IsBusy)
            {
                BW_generator.RunWorkerAsync();
            }
        }

        private void generator_Unchecked(object sender, RoutedEventArgs e)
        {
            if (BW_generator.IsBusy)
            {
                BW_generator.CancelAsync();
            }
        }

        // *****************
        // **** SHIELDS ****
        // *****************

        private void shields_Checked(object sender, RoutedEventArgs e)
        {
            EnergyModule.setShields();
        }

        private void shields_Unchecked(object sender, RoutedEventArgs e)
        {
            EnergyModule.setShields();
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
        }

        // **************
        // **** FUEL ****
        // **************

        private void Fuel_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                int fuelLevel = MovementModule.getFuelLevel();
                BW_fuel.ReportProgress(fuelLevel);
                System.Threading.Thread.Sleep(1000);
            }
        }

        private void Fuel_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            fuel.Value = e.ProgressPercentage;
            fuelText.Text = "Fuel Level: " + e.ProgressPercentage.ToString() + "%";
        }


        // ***************
        // **** SPEED ****
        // ***************

        private void speedChanger_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MovementModule.speed = e.NewValue;
            speedText.Text = e.NewValue + "km/s";

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
            BW_damage.WorkerSupportsCancellation = true;
            BW_damage.DoWork += Damage_DoWork;
        }

        // ****************
        // ***** HULL *****
        // ****************

        private void Hull_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                int hullIntegrity = ExteriorIntegrityModule.getHullIntegrity();
                BW_hull.ReportProgress(hullIntegrity);
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
            while (true)
            {
                float newHullIntegrity = ExteriorIntegrityModule.getHullIntegrity() - (0.01 * MovementModule.getSpeed());
                ExteriorIntegrityModule.setHullIntegrity();
                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}
