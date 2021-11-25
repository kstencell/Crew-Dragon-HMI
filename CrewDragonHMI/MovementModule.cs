using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

namespace CrewDragonHMI
{
    public static class MovementModule
    {
        private static int speed, direction;
        private static float fuelLevel;
        private static string fuelFilePath = "FuelLevel.txt", speedFilePath = "Speed.txt", directionFilePath = "Direction.txt";
        private static bool warpDriveStatus;

        static MovementModule()
        {
            setFuelLevel(100.0F);
            setSpeed(0);
            setDirection(0);
            warpDriveStatus = false;
        }

        public static float getFuelLevel()
        {
            try
            {
                StreamReader fuelLevelStreamReader = new StreamReader(fuelFilePath);
                fuelLevel = float.Parse(fuelLevelStreamReader.ReadLine(), CultureInfo.InvariantCulture.NumberFormat);
                fuelLevelStreamReader.Close();
            }
            catch (IOException)
            {
                Thread.Sleep(50);
            }

            return fuelLevel;
        }

        private static void setFuelLevel(float fuelLevel)
        {
            try
            {
                StreamWriter fileStream = new StreamWriter(fuelFilePath);
                fileStream.WriteLine((fuelLevel).ToString());
                fileStream.Close();
            }

            catch (IOException)
            {
                Thread.Sleep(50);
            }
        }

        public static bool requestFuel(float fuelRequested)
        {
            float newFuelLevel = getFuelLevel() - fuelRequested;

            if (newFuelLevel >= 0)
            {
                setFuelLevel(newFuelLevel);
                return true;
            }
            else
            {
                return false;
            }
            
        }

        public static int getSpeed()
        {
            try
            {
                StreamReader speedStreamReader = new StreamReader(speedFilePath);
                speed = Int32.Parse(speedStreamReader.ReadLine());
                speedStreamReader.Close();
            }

            catch (IOException)
            {
                Thread.Sleep(50);
            }

            return speed;
        }

        public static void setSpeed(int speed)
        {
            try
            {
                StreamWriter fileStream = new StreamWriter(speedFilePath);
                fileStream.WriteLine((speed).ToString());
                fileStream.Close();
            }

            catch (IOException)
            {
                Thread.Sleep(50);
            }
        }

        public static bool requestSpeedChange(int newSpeed)
        {
            int speedDifference = Math.Abs(newSpeed - getSpeed());
            int fuelRequired = speedDifference / 100;
            if (requestFuel(fuelRequired))
            {
                setSpeed(newSpeed);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static int getDirection()
        {
            try
            {
                StreamReader directionStreamReader = new StreamReader(directionFilePath);
                direction= Int32.Parse(directionStreamReader.ReadLine());
                directionStreamReader.Close();
            }
            catch (IOException)
            {
                Thread.Sleep(50);
            }

            return direction;
        }

        private static void setDirection(int newDirection)
        {
            try
            {
                StreamWriter fileStream = new StreamWriter(directionFilePath);
                fileStream.WriteLine((newDirection).ToString());
                fileStream.Close();
            }

            catch (IOException)
            {
                Thread.Sleep(50);
            }
        }

        public static bool requestDirectionChange(int newDirection)
        {
            float directionDifference = Math.Abs(newDirection - getDirection());
            float fuelRequired = directionDifference / 360;
            if (requestFuel(fuelRequired))
            {
                setDirection(newDirection);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void toggleWarpDrive()
        {
            warpDriveStatus = !warpDriveStatus;
        }

        public static bool getWarpDriveStatus()
        {
            return warpDriveStatus;
        }
    }
}
