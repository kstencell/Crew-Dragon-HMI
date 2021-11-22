using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrewDragonHMI
{
    class CriticalAlert
    {
        public bool isActive { get; private set; }
        public Dictionary<string, bool> onAlert { get; private set; }
        public Dictionary<string, int> alertThresholds { get; private set; }
        public string alertMessage { get; private set; } // GOAL

        public CriticalAlert(string fileName, string[] moduleNames)
        {

        }


    }
}
