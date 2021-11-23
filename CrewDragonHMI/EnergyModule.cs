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
        public static bool shieldStatus;


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

        private static void setBatteryLevel(int initialLevel)
        {
            batteryLevel = initialLevel;
        }

        public static bool requestEnergy(int energyRequested)
        {
            int newBatteryLevel = getBatteryLevel() - energyRequested;

            if (newBatteryLevel >= 0)
            {
                try
                {
                    StreamWriter fileStream = new StreamWriter(batteryFilePath);
                    fileStream.WriteLine(newBatteryLevel.ToString());
                    fileStream.Close();
                    return true;
                }
                catch (IOException)
                {
                    Thread.Sleep(50);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static bool getGeneratorStatus()
        {
            return generatorStatus;
        }

        public static void toggleGeneratorStatus()
        {
            generatorStatus = !generatorStatus;
        }

        public static bool getShieldStatus()
        {
            return shieldStatus;
        }

        public static void toggleShieldStatus(bool status)
        {
            shieldStatus = !shieldStatus;
        }
    }
}
