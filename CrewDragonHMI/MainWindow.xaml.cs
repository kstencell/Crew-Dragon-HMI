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
            batteryText.Text = "BATTERY: " + e.ProgressPercentage.ToString() + "%";
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
            this.Dispatcher.Invoke(() =>
            {
                alarm.IsChecked = true;
                alarm.IsEnabled = false;
            });

            BW_alert.DoWork += Alert_DoWork;
            BW_alert.RunWorkerAsync();
        }

        // Alarm toggle button is only active during active alerts
        private void alarm_Checked(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                alarm.Background = Brushes.Red;
                alarmText.Foreground = Brushes.White;
            });
        }

        private void alarm_Unchecked(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                alarm.Background = Brushes.Gray;
                alarmText.Foreground = Brushes.Black;
            });
        }



        private void Alert_DoWork(object sender, DoWorkEventArgs e)
        {
            while(true)
            {
                Dictionary<string, int> sensorValues = new Dictionary<string, int>();
                sensorValues["Hull"] = (int) ExteriorIntegrityModule.getHullIntegrity();
                sensorValues["Fuel"] = (int) MovementModule.getFuelLevel();
                sensorValues["Battery"] = EnergyModule.getBatteryLevel();

                foreach (KeyValuePair<string, int> pair in sensorValues)
                {
                    AlertModule.ReceiveSensorValue(pair.Key, pair.Value);
                }

                bool isOnAlert = AlertModule.ReadAlert();



                this.Dispatcher.Invoke(() =>
                {
                    if (isOnAlert)
                    {
                        if (!alarm.IsEnabled) // Enable dismissal if possible
                        {
                            alarm.IsEnabled = true;
                        }

                        this.Dispatcher.Invoke(() =>
                        {
                            alarmText.Content = "SHIP STATUS: ALERT";
                            
                            if ((bool) alarm.IsChecked)
                            {
                                alarm.Background = Brushes.Red;
                            }
                            else
                            {
                                alarm.Background = Brushes.Gray;
                            }
                            
                        });
                    }
                    else
                    {
                        alarm.IsEnabled = false;
                        alarm.IsChecked = true; // Reset "snooze"
                        alarm.Background = Brushes.Green;

                        alarmText.Content = "SHIP STATUS: FUNCTIONAL";
                        alarmText.Foreground = Brushes.White;
 
                    }


                });

                System.Threading.Thread.Sleep(1000);
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
            hullText.Text = e.ProgressPercentage.ToString() + "%";
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
            
            for (int numFlashes = 5; numFlashes > 0; numFlashes--)
            {
                this.Dispatcher.Invoke(() =>
                {
                    evacuateText.Visibility = Visibility.Visible;
                });

                System.Threading.Thread.Sleep(2000);

                this.Dispatcher.Invoke(() =>
                {
                    evacuateText.Visibility = Visibility.Hidden;
                });

                System.Threading.Thread.Sleep(1000);
            }
           

            Environment.Exit(0);
        }

        private void alarm_Click(object sender, RoutedEventArgs e)
        {

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

            fuelText.Text = "FUEL: " + e.ProgressPercentage.ToString() + "%";
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
                    speedText.Text = "SPEED: " + (int)speedSlider.Value + " KM/S";
                }
                else
                {
                    speedSlider.Value = MovementModule.getSpeed();
                    speedText.Text = "SPEED: " + (int)speedSlider.Value + " KM/S";
                }
            }
            else
            {
                speedSlider.Value = MovementModule.getSpeed();
                speedText.Text = "SPEED: " + (int)speedSlider.Value + " KM/S";
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
                    speedText.Text = "SPEED: " + (int)speedSlider.Value + " KM/S";
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
                    speedText.Text = "SPEED: " + (int)speedSlider.Value + " KM/S";
                });

            }
        }

        // ***********************
        // ****** DIRECTION ******
        // ***********************

        private bool _isPressed = false;
        private Canvas _templateCanvas = null;

        private void Ellipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Enable moving mouse to change the value.
            if (!MovementModule.getWarpDriveStatus())
            {
                _isPressed = true;
            }
        }

        private void Ellipse_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //Disable moving mouse to change the value.
            _isPressed = false;
        }

        private void Ellipse_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isPressed)
            {
                //Find the parent canvas.
                if (_templateCanvas == null)
                {
                    _templateCanvas = MyHelper.FindParent<Canvas>(e.Source as Ellipse);
                    if (_templateCanvas == null) return;
                }
                //Calculate the current rotation angle and set the value.
                const double RADIUS = 150;
                Point newPos = e.GetPosition(_templateCanvas);
                double angle = MyHelper.GetAngleR(newPos, RADIUS);
                knob.Value = (knob.Maximum - knob.Minimum) * angle / (2 * Math.PI);

                this.Dispatcher.Invoke(() =>
                {
                    rotationText.Text = "DIRECTION: " +  ((int)(angle * 180.0 / Math.PI)).ToString() + "°";
                });
            }
        }
    }

    //The converter used to convert the value to the rotation angle.
    public class ValueAngleConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter,
                      System.Globalization.CultureInfo culture)
        {
            double value = (double)values[0];
            double minimum = (double)values[1];
            double maximum = (double)values[2];

            return MyHelper.GetAngle(value, maximum, minimum);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter,
              System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    //Convert the value to text.
    public class ValueTextConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
                  System.Globalization.CultureInfo culture)
        {
            double v = (double)value;
            return String.Format("{0:F2}", v);
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public static class MyHelper
    {
        //Get the parent of an item.
        public static T FindParent<T>(FrameworkElement current)
          where T : FrameworkElement
        {
            do
            {
                current = VisualTreeHelper.GetParent(current) as FrameworkElement;
                if (current is T)
                {
                    return (T)current;
                }
            }
            while (current != null);
            return null;
        }

        //Get the rotation angle from the value
        public static double GetAngle(double value, double maximum, double minimum)
        {
            double current = (value / (maximum - minimum)) * 360;
            if (current == 360)
                current = 359.999;

            return current;
        }

        //Get the rotation angle from the position of the mouse
        public static double GetAngleR(Point pos, double radius)
        {
            //Calculate out the distance(r) between the center and the position
            Point center = new Point(radius, radius);
            double xDiff = center.X - pos.X;
            double yDiff = center.Y - pos.Y;
            double r = Math.Sqrt(xDiff * xDiff + yDiff * yDiff);

            //Calculate the angle
            double angle = Math.Acos((center.Y - pos.Y) / r);
            Console.WriteLine("r:{0},y:{1},angle:{2}.", r, pos.Y, angle);
            if (pos.X < radius)
                angle = 2 * Math.PI - angle;
            if (Double.IsNaN(angle))
                return 0.0;
            else
                return angle;
        }
    }
}
