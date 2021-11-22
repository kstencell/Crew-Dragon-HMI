using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using System.Collections;

namespace CrewDragonHMI
{
    class AlertConfig
    {
        public int BatteryThreshold { get; set; }
        public int HullThreshold { get; set; }
        public int FuelThreshold { get; set; }
    }


    static class AlertModule
    {
        static public bool isActive { get; private set; }
        static public Dictionary<string, bool> onAlert { get; private set; }
        static public Dictionary<string, int> alertThresholds { get; private set; }
        static public string alertMessage { get; private set; } // GOAL


        static public string alertFile { get; private set; }
        static public string configFile { get; private set; }

        static AlertModule()
        {
            alertFile = "alertStatus.txt";
            configFile = "alertConfig.json";

            string configText = File.ReadAllText(configFile);

            AlertConfig alertConfig = JsonSerializer.Deserialize<AlertConfig>(configText);

            alertThresholds["BatteryThreshold"] = alertConfig.BatteryThreshold;
            alertThresholds["HullThreshold"] = alertConfig.HullThreshold;
            alertThresholds["FuelThreshold"] = alertConfig.FuelThreshold;

            onAlert["BatteryThreshold"] = false;
            onAlert["HullThreshold"] = false;
            onAlert["FuelThreshold"] = false;
        }

        static private void UpdateAlertFile()
        {
            bool isOnAlert = false;
            foreach (KeyValuePair<string, bool> entry in onAlert)
            {
                if (entry.Value)
                {
                    isOnAlert = true;
                    break;
                }
            }

            WriteAlert(isOnAlert);
        }

        static private void WriteAlert(bool isOnAlert)
        {
            using (StreamWriter sw = File.CreateText(alertFile))
            {
                if (isOnAlert)
                {
                    sw.WriteLine("yesalert");
                }
                else
                {
                    sw.WriteLine("noalert");
                }
            }
        }

        static private bool ReadAlert()
        {
            using (StreamReader sr = File.OpenText(alertFile))
            {
                string read_line = sr.ReadLine();

                if (read_line == "yesalert")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
