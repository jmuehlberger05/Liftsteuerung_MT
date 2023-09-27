using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Liftsteuerung_MT
{

    internal class Person
    {

        private Random _random;
        private Elevator _elevator;

        public int? CurrentLevel { get; set; }
        public int? DesiredLevel { get; set; }
        //public Boolean DesireToChangeLevel { get; set; }

        public string ThreadName { get; set; }


        public Person(Elevator elevator, Random random) {
            _elevator = elevator;
            _random = random;

            //DesireToChangeLevel = false;


            // Set Persons Location to a random Level
            CurrentLevel = _random.Next(1, Constants.MAX_LEVEL);

            //Console.WriteLine($"CurrentLevel: {CurrentLevel}");

            //Console.WriteLine($"Desired Level: {decideForNewLevel()}");
        }

        public void changeLevel(){
            //if (CurrentLevel == DesiredLevel) return;
            ThreadName = Thread.CurrentThread.Name;
            string name = $"Person-{Thread.CurrentThread.Name}";
            _elevator.AddToAllPeople(this);

            for (int i = 0; i < Constants.MAX_LEVEL_CHANGE_CYCLES; i++)
            {
                if (CurrentLevel != DesiredLevel)
                {
                    logToConsole($"· {name} is on Level {CurrentLevel}", ConsoleColor.Blue);
                }

                // Wait for a random Amount of Time
                Thread.Sleep(decideForWaitingDuration());


                // Check for new Level and then see if they are the same
                if (CurrentLevel == decideForNewLevel()) continue;

                // Decide for a new Level to switch to
                logToConsole($"? {name} wants to switch from Level {CurrentLevel} {evaualteUpOrDown(CurrentLevel, DesiredLevel)} to Level {DesiredLevel}", ConsoleColor.Red);

                // Wait for the Elevator to be on their floor and moving in the right direction
                while (_elevator.CurrentLevel != CurrentLevel || (_elevator.Direction != evaualteUpOrDown(CurrentLevel, DesiredLevel) && _elevator.Direction != Constants.ELEVATOR_DIRECTIONS["NONE"]))
                {
                    Thread.Sleep(1000); // Polling interval
                }

                // Wait for the Eleveator and then get in
                _elevator.Semaphore.WaitOne();
                _elevator.AddPerson(this);
                logToConsole($"$ {name} is in the Elevator", ConsoleColor.Yellow);
                logToConsole($"Currently there are {_elevator.GetPeopleOnElevatorCount()} People on the Elevator", ConsoleColor.Cyan);

                /* switch to new Level */
                // ! TEMPORARY !
                CurrentLevel = DesiredLevel;
                DesiredLevel = null;
                Thread.Sleep(3000);

                _elevator.Semaphore.Release();
                _elevator.RemovePerson(this);

            }
            
        }

        // Returns the new Desired Level for the Person to go to
        private int? decideForNewLevel() { 
           return DesiredLevel = _random.Next(1, Constants.MAX_LEVEL);
        }

        // Returns a Random Time in ms, that the Person will spend on the Level
        private int decideForWaitingDuration(){
            return _random.Next(Constants.MIN_WAITING_DURATION, Constants.MAX_WAITING_DURATION);
        }

        private string evaualteUpOrDown(int? currentLevel, int? desiredLevel) => desiredLevel > currentLevel ? "up" : "down";

        // Makes writung to the Console in Colors easier
        private static readonly object colorLock = new object();
        private void logToConsole(string message, ConsoleColor color)
        {
            lock (colorLock)
            {
                Console.ForegroundColor = color;
                Console.WriteLine(message);
                Console.ResetColor();
            }
        }

        public string GetDesireSymbol()
        {
            if (!DesiredLevel.HasValue)
                return "·";

            if (DesiredLevel > CurrentLevel)
                return "up";

            if (DesiredLevel < CurrentLevel)
                return "down";

            return "·";  // By default, we'll assume they're happy.
        }


    }
}