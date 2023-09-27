using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Liftsteuerung_MT
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Person[] persons = new Person[Constants.MAX_PERSON_COUNT];
            Thread[] threads = new Thread[Constants.MAX_PERSON_COUNT];

            Elevator elevator = new Elevator();

            Random random = new Random();

            Thread elevatorThread = new Thread(elevator.MoveAndService);
            elevatorThread.Start();

            // Initialize Persons
            for (int i = 0; i < Constants.MAX_PERSON_COUNT; i++)
            {
                // Person und Thread erstellen und den Namen ändern
                persons[i] = new Person(elevator, random);

                Thread _thread = new Thread(persons[i].changeLevel);
                _thread.Name = $"{i}";
                _thread.Start();

                threads[i] = _thread;
            }

            Console.ReadKey();
        }
    }
}
