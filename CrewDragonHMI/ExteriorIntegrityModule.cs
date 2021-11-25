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
    static class ExteriorIntegrityModule
    {
        private static float hullIntegrity;
        private static string hullFilePath = "Hull.txt";

        static ExteriorIntegrityModule()
        {
            setHullIntegrity(100.0f);
        }

        public static float getHullIntegrity()
        {
            try
            {
                StreamReader hullStreamReader = new StreamReader(hullFilePath);
                hullIntegrity = float.Parse(hullStreamReader.ReadLine(), CultureInfo.InvariantCulture.NumberFormat);
                hullStreamReader.Close();
            }
            catch (IOException)
            {
                Thread.Sleep(50);
            }

            return hullIntegrity;
        }

        private static void setHullIntegrity(float hull)
        {
            try
            {
                StreamWriter fileStream = new StreamWriter(hullFilePath);
                fileStream.WriteLine(hull.ToString());
                fileStream.Close();
            }
            catch (IOException)
            {
                Thread.Sleep(50);
            }
        }

        public static void takeDamage(float damage)
        {
            if (!EnergyModule.getShieldStatus())
            {
                setHullIntegrity(getHullIntegrity() - damage);
            }
        }
    }
}
