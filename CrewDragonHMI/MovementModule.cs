using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace CrewDragonHMI
{
    public static class MovementModule
    {
        public static int fuelLevel;
        public static string fuelFilePath = "FuelLevel.txt";

        static MovementModule()
        {
            setFuelLevel(100);
        }

        public static int getFuelLevel()
        {
            try
            {
                StreamReader fuelLevelStreamReader = new StreamReader(fuelFilePath);
                fuelLevel = Int32.Parse(fuelLevelStreamReader.ReadLine());
                fuelLevelStreamReader.Close();
            }

            catch (IOException)
            {
                Thread.Sleep(50);
            }

            return fuelLevel;
        }

        public static void setFuelLevel(int fuelLevel)
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
    }
}
