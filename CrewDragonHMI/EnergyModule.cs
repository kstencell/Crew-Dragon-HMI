using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel;
using System.Globalization;

namespace CrewDragonHMI
{
    public static class EnergyModule
    {

        private static float batteryLevel;
        private static string batteryFilePath = "BatteryLevel.txt";
        public static bool generatorStatus;
        public static bool shieldStatus;


        static EnergyModule ()
        {
            setBatteryLevel(10.0F);

        }

        public static int getBatteryLevel()
        {
            try
            {
                StreamReader batteryLevelStreamReader = new StreamReader(batteryFilePath);
                batteryLevel = float.Parse(batteryLevelStreamReader.ReadLine(), CultureInfo.InvariantCulture.NumberFormat);
                batteryLevelStreamReader.Close();
            } catch (IOException)
            {
                Thread.Sleep(50);
            }

            return (int)batteryLevel;
        }

        private static void setBatteryLevel(float level)
        {
            try
            {
                StreamWriter fileStream = new StreamWriter(batteryFilePath);
                fileStream.WriteLine((level).ToString());
                fileStream.Close();
            }

            catch (IOException)
            {
                Thread.Sleep(50);
            }
        }

        public static bool requestEnergy(float energyRequested)
        {
            float newBatteryLevel = getBatteryLevel() - energyRequested;

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

        public static void generateEnergy()
        {
            if (batteryLevel <= 99)
            {
                setBatteryLevel(batteryLevel + 1);
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

        public static void toggleShieldStatus()
        {
            shieldStatus = !shieldStatus;
        }
    }
}
