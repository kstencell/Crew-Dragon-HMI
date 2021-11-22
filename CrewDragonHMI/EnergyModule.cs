using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrewDragonHMI
{
    static class EnergyModule
    {
        public static int BatteryLevel { get; set; }

        public static void SetShields()
        {

        }

        static EnergyModule()
        {
            BatteryLevel = 50;
        }
    }
}
