using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSystemLibrary
{
    public class Utility
    {
        public enum Direction
        {
            Up, 
            Down, 
            NoDirection /* When Elevator is stopped in a floor and it is not moving either Up or Down. */
        }
        public static int TopFloorID { get; set; }
        public static int BottomFloorID { get; set; }
        public static int CrossTime { get; set; }
        public static int HaltTime { get; set; }
        public static Queue<int> RequestedFloorLobbies { get; set; }
        public static IDictionary<Elevator, int> ElevatorReqFloorTimeMap { get; set; }
        public static IList<AbstractFloorLobby> FloorLobbies { get; set; }
        public static IList<Elevator> Elevators { get; set; }
        public static int TotalElevators { get; set; }
        public static int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

        /// <summary>
        ///  Find the direction(up/down) of an elevator when travel from current floor to destination floor.
        /// </summary>
        /// <param name="currentFloor">Current floor</param>
        /// <param name="requestedFloor">Destination floor</param>
        /// <returns></returns>
        public static Direction GetElevatorDirection(int currentFloor, int requestedFloor)
        {
            if(currentFloor == requestedFloor)
            {
                return Direction.NoDirection;
            }
            if(currentFloor == Utility.TopFloorID)
            {
                return Direction.Down;
            }
            else if(currentFloor == Utility.BottomFloorID)
            {
                return Direction.Up;
            }
            else if( currentFloor >=0 && requestedFloor >= 0)
            {
                return requestedFloor - currentFloor >=0 ?  Direction.Up : Direction.Down;
            }
            else if(currentFloor >= 0 && requestedFloor <= 0)
            {
                return Direction.Down;
            }
            else if(currentFloor <= 0 && requestedFloor >=0)
            {
                return Direction.Up;
            }
            else
            {
                return currentFloor - requestedFloor >= 0 ? Direction.Down : Direction.Up;
            }

        }

        /// <summary>
        /// Time taken by an elevator to reach to particular floor from it's current position.
        /// </summary>
        /// <param name="elevator">Pass the elevator you want to find out time to reach particular floor</param>
        /// <param name="requstedFloorNo">Which floor you want to travel to</param>
        /// <returns></returns>
        public static int FindTimeTaken(Elevator elevator, int requstedFloorNo)
        {

            int timeTaken = 0;
            int currentFloorNo = elevator.CurrentFloorNo;
            Utility.Direction direction = currentFloorNo > requstedFloorNo ? Utility.Direction.Down : Utility.Direction.Up;
            if (currentFloorNo == requstedFloorNo)
            {
                return 0;
            }
            if (elevator.ElevatorDirection == direction || elevator.ElevatorDirection == Utility.Direction.NoDirection)
            {
                if (elevator.ElevatorDirection == Utility.Direction.NoDirection)
                {
                    elevator.ElevatorDirection = direction;
                }
                /* if the elevaor direction is down and requested follor is below the current floor  
                *  or if elevatordirection is up and requested floor is above current floor.
                */
                while (currentFloorNo != requstedFloorNo)
                {
                    if (elevator.FloorsStopMap[currentFloorNo] == true)
                    {
                        timeTaken += Utility.CrossTime + Utility.HaltTime;
                    }
                    else
                    {
                        timeTaken += Utility.CrossTime;
                    }
                    if (direction == Utility.Direction.Up)
                    {
                        currentFloorNo++;
                    }
                    else if (direction == Utility.Direction.Down)
                    {
                        currentFloorNo--;
                    }

                }
            }
            else
            {
                /* if the elevaor direction is up and requested follor is below the current floor or if elevator direction is down and requested floor is above current floor */
                KeyValuePair<int, bool> lastStop;
                if (elevator.ElevatorDirection == Utility.Direction.Up)
                {
                    lastStop = elevator.FloorsStopMap.LastOrDefault(x => x.Value == true);
                }
                else
                {
                    lastStop = elevator.FloorsStopMap.FirstOrDefault(x => x.Value == true);
                }

                while (currentFloorNo != lastStop.Key && currentFloorNo > Utility.BottomFloorID && currentFloorNo < Utility.TopFloorID)
                {
                    if (elevator.FloorsStopMap[currentFloorNo] == true)
                    {
                        timeTaken += Utility.CrossTime + Utility.HaltTime;
                    }
                    else
                    {
                        timeTaken += Utility.CrossTime;
                    }
                    if (elevator.ElevatorDirection == Utility.Direction.Up)
                    {
                        currentFloorNo = currentFloorNo > requstedFloorNo ? currentFloorNo++ : currentFloorNo--;
                    }
                    else if (elevator.ElevatorDirection == Utility.Direction.Down)
                    {
                        currentFloorNo = currentFloorNo > requstedFloorNo ? currentFloorNo-- : currentFloorNo++;
                    }

                }
                currentFloorNo = lastStop.Key;
                while (currentFloorNo != requstedFloorNo && currentFloorNo >= Utility.BottomFloorID && currentFloorNo <= Utility.TopFloorID)
                {
                    if (elevator.FloorsStopMap[currentFloorNo] == true)
                    {
                        timeTaken += Utility.CrossTime + Utility.HaltTime;
                    }
                    else
                    {
                        timeTaken += Utility.CrossTime;
                    }
                    currentFloorNo = currentFloorNo > requstedFloorNo ? currentFloorNo-- : currentFloorNo++;
                }
            }

            return timeTaken;
        }

        /// <summary>
        /// Find the nearest elevator from the requested floor lobby when a up/down button is pressed by a person
        /// </summary>
        /// <param name="floor">This is the floor number i.e. form which floor lobby the user is pressed the button. </param>
        /// <param name="elevators">All availabe elevators to the Elevator system.</param>
        /// <returns>Returns the nearest elevator.</returns>
        public static IDictionary<Elevator, int> GetElevatorTimeMap(int requestedFloorNo)
        {
            var elevatorTimeMap = new Dictionary<Elevator, int>();

            System.Threading.Tasks.Task task = System.Threading.Tasks.Task.Run(() => Elevators.ToList().ForEach(elevator =>
            {
                int t = Utility.FindTimeTaken(elevator, requestedFloorNo);
                elevatorTimeMap.Add(elevator, t);
            }));
            task.Wait();
            return elevatorTimeMap;
        }
    }
}
