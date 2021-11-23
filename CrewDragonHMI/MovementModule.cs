using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrewDragonHMI
{
    static class MovementModule
    {
        public static int FuelLevel { get; set; }
        public static int Speed { get; set; }
    
        static MovementModule()
        {
            FuelLevel = 50;
            Speed = 50;
        }

    }
}
