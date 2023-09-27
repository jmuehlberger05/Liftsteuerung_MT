using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liftsteuerung_MT
{
    internal class Constants
    {
        public const int MAX_LEVEL = 5;
        public const int MAX_PERSON_COUNT = 10;
        public const int MAX_PERSON_COUNT_IN_ELEVATOR = 3;

        public const int MAX_WAITING_DURATION = 10000;
        public const int MIN_WAITING_DURATION = 2000;

        public const int MAX_LEVEL_CHANGE_CYCLES = 3;

        public static readonly Dictionary<string, string> ELEVATOR_DIRECTIONS = new Dictionary<string, string> {
            ["UP"] = "up",
            ["DOWN"] = "down",
            ["NONE"] = "none"
        };
    }
}
