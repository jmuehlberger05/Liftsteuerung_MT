using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Liftsteuerung_MT
{
    internal class Elevator
    {

        public int CurrentLevel { get; set; }

        public int NextStop { get; set; }

        public string Direction { get; set; }

        public Semaphore Semaphore { get; set; }

        public List<Person> PeopleOnElevator { get; set; }
        public static List<Person> AllPeople = new List<Person>();


        public Elevator()
        {
            CurrentLevel = 0;

            PeopleOnElevator = new List<Person>();

            Semaphore = new Semaphore(
                initialCount: Constants.MAX_PERSON_COUNT_IN_ELEVATOR,
                maximumCount: Constants.MAX_PERSON_COUNT_IN_ELEVATOR
                );

            Direction = Constants.ELEVATOR_DIRECTIONS["NONE"];
        }

        public void AddPerson(Person person)
        {
            PeopleOnElevator.Add(person);
        }

        public void AddToAllPeople(Person person)
        {
            AllPeople.Add(person);
        }

        public void RemovePerson(Person person)
        {
            if (PeopleOnElevator.Contains(person))
                PeopleOnElevator.Remove(person);
        }

        public int GetPeopleOnElevatorCount()
        {
            return PeopleOnElevator.Count();
        }

        public void MoveAndService()
        {
            while (true) // this makes the elevator always moving and servicing
            {
                DisplayBuilding(); // Display the building at each iteration


                // Decide direction
                if (CurrentLevel == Constants.MAX_LEVEL - 1)
                {
                    Direction = Constants.ELEVATOR_DIRECTIONS["DOWN"];
                }
                else if (CurrentLevel == 0)
                {
                    Direction = Constants.ELEVATOR_DIRECTIONS["UP"];
                }

                // Sleep to simulate elevator moving time (adjust as needed)
                Thread.Sleep(2000);

                if (Direction == Constants.ELEVATOR_DIRECTIONS["UP"])
                {
                    CurrentLevel++;
                }
                else if (Direction == Constants.ELEVATOR_DIRECTIONS["DOWN"])
                {
                    CurrentLevel--;
                }

                Service();

                LogElevatorStatus();

            }
        }

        private void LogElevatorStatus()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Elevator Info: Current Level = {CurrentLevel}, Direction = {Direction}, PeopleOnElevator count inside = {PeopleOnElevator.Count}");
            Console.ResetColor();
        }

        public void Service()
        {
            if (PeopleOnElevator == null)
            {
                Console.WriteLine("PeopleOnElevator list is null!");
                return; // Exit the method if PeopleOnElevator is null.
            }

            // Allow PeopleOnElevator to get off first
            for (int i = PeopleOnElevator.Count - 1; i >= 0; i--)
            {
                if (PeopleOnElevator[i] == null)
                {
                    Console.WriteLine("PeopleOnElevator list is null!");
                    return; // Exit the method if PeopleOnElevator is null.
                }

                if (PeopleOnElevator[i].DesiredLevel == CurrentLevel)
                {
                    PeopleOnElevator[i].CurrentLevel = CurrentLevel;
                    PeopleOnElevator[i].DesiredLevel = null;
                    Semaphore.Release();  // person gets off, so we release the semaphore
                    PeopleOnElevator.RemoveAt(i);
                }
            }

            // Attempt to let people in the elevator, if there's space
            var peopleWaitingAtCurrentLevel = PeopleOnElevator.Where(p => p.CurrentLevel == CurrentLevel &&
                                                        p.DesiredLevel.HasValue &&
                                                        ((Direction == "Up" && p.CurrentLevel < p.DesiredLevel) ||
                                                         (Direction == "Down" && p.CurrentLevel > p.DesiredLevel))).ToList();

            foreach (var person in peopleWaitingAtCurrentLevel)
            {
                if (PeopleOnElevator == null)
                {
                    Console.WriteLine("PeopleOnElevator list is null!");
                    return; // Exit the method if PeopleOnElevator is null.
                }
                if (PeopleOnElevator.Count < Constants.MAX_PERSON_COUNT_IN_ELEVATOR) // Check if the elevator is not full
                {
                    PeopleOnElevator.Add(person);  // person gets in the elevator
                    Semaphore.WaitOne();  // so we acquire the semaphore for this person
                }
                else
                {
                    break;  // if elevator is full, no more people can get in
                }
            }
        }

        public void DisplayBuilding()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = Constants.MAX_LEVEL - 1; i >= 0; i--)
            {
                sb.Append(i).Append(". ");

                // Display the elevator
                if (CurrentLevel == i)
                {
                    sb.Append('[');

                    int peopleInElevator = PeopleOnElevator.Count;
                    for (int j = 0; j < Constants.MAX_PERSON_COUNT_IN_ELEVATOR; j++)
                    {
                        
                        if (j < peopleInElevator && PeopleOnElevator[j] != null)
                        {
                            sb.Append(PeopleOnElevator[j].ThreadName);
                        }
                        else
                        {
                            sb.Append('.');
                        }
                    }

                    sb.Append(']');
                }
                else
                {
                    sb.Append("|   |");
                }

                // Add people who are on this level but not inside the elevator
                sb.Append(" - ");
                foreach (var person in PeopleWaitingOnFloor(i)) // We'll create this function next
                {
                    sb.Append(person.ThreadName[person.ThreadName.Length - 1])
                      .Append(person.GetDesireSymbol())
                      .Append(", ");

                }

                // Remove trailing comma and space, if present
                if (sb.ToString().EndsWith(", "))
                    sb.Length -= 2;

                sb.AppendLine();
            }
            Console.WriteLine(sb.ToString());
        }

        // This function gets people waiting on a specific floor.
        public List<Person> PeopleWaitingOnFloor(int floor)
        {

            if (AllPeople != null)
                return AllPeople.Where(p => p.CurrentLevel == floor).ToList();

            return new List<Person>();
        }


    }
}
