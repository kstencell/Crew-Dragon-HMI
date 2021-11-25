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
        private static int hullIntegrity;
        private static string hullFilePath = "Hull.txt";

        static ExteriorIntegrityModule()
        {
            setHullIntegrity(100);
        }

        public static int getHullIntegrity()
        {
            try
            {
                StreamReader hullStreamReader = new StreamReader(hullFilePath);
                hullIntegrity = Int32.Parse(hullStreamReader.ReadLine());
                hullStreamReader.Close();
            }
            catch (IOException)
            {
                Thread.Sleep(50);
            }

            return hullIntegrity;
        }

        private static void setHullIntegrity(int hull)
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

        public static void takeDamage()
        {
            if (!EnergyModule.getShieldStatus())
            {
                setHullIntegrity(getHullIntegrity() - 10);
            }
        }
    }
}
