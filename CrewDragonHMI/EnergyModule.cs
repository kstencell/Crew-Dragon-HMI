using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel;

namespace CrewDragonHMI
{
    public static class EnergyModule
    {

        public static int batteryLevel;
        public static string batteryFilePath = "BatteryLevel.txt";
        public static bool generatorStatus;
        public static bool shieldsStatus;


        static EnergyModule ()
        {
            setBatteryLevel(50);

        }

        public static int getBatteryLevel()
        {
            try
            {
                StreamReader batteryLevelStreamReader = new StreamReader(batteryFilePath);
                batteryLevel = Int32.Parse(batteryLevelStreamReader.ReadLine());
                batteryLevelStreamReader.Close();
            } catch (IOException)
            {
                Thread.Sleep(50);
            }

            return batteryLevel;
        }

        public static void setBatteryLevel(int batteryLevel)
        {
            try
            {
                StreamWriter fileStream = new StreamWriter(batteryFilePath);
                fileStream.WriteLine((batteryLevel).ToString());
                fileStream.Close();
            }
            catch (IOException)
            {
                Thread.Sleep(50);
            }
        }

        public static bool getGeneratorStatus()
        {
            return generatorStatus;
        }

        public static void setGeneratorStatus(bool status)
        {
            generatorStatus = status;
        }

        public static bool getShieldStatus()
        {
            return shieldsStatus;
        }

        public static void setShieldStatus(bool status)
        {
            shieldsStatus = generatorStatus;
        }
    }
}
