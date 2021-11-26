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
                int batteryLevel = EnergyModule.batteryLevel;
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
                EnergyModule.batteryLevel = EnergyModule.batteryLevel + 1; // Having this variable completely public makes me queasy, but it is OK for now
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
            //EnergyModule.SetShields(); // Double check this with requirements. Seems like a bool should be the parameter
        }

        private void shields_Unchecked(object sender, RoutedEventArgs e)
        {
            //EnergyModule.SetShields();
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
                int fuelLevel = MovementModule.FuelLevel;
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
            MovementModule.Speed = (int)e.NewValue; // I lazily cast this as an int. Should Speed be an int?
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
                float hullIntegrity = ExteriorIntegrityModule.HullIntegrity;
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
            while (true)
            {
                float newHullIntegrity = ExteriorIntegrityModule.HullIntegrity - (0.01F * MovementModule.Speed);
                ExteriorIntegrityModule.HullIntegrity = newHullIntegrity;
                System.Threading.Thread.Sleep(1000);
            }
        }


        // ******************
        // ****** DIAL ******
        // ******************


        private bool _isPressed = false;
        private Canvas _templateCanvas = null;

        private void Ellipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Enable moving mouse to change the value.
            _isPressed = true;
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
                    rotationText.Text = ((int)((angle * 180)/Math.PI)).ToString();
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
