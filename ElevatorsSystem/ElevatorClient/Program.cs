using ElevatorSystemLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
//using System.Threading.Tasks;
using System.Timers;

namespace ElevatorClient
{
    class Program
    {
        static void Main(string[] args)
        {
            /*Below are the assumption taken, This can be read from console by user input*/
            int noOfBasement = 2; /* Total number of basement flooor present */
            int noOfFloors = 10; /* Total number of floors avove ground floor */
            int noOfElevators = 3;
            int timeToCrossAFloor = 2;/* Assume 2 seconds needs to cross a floor for an elevator */
            int haltTimeatAFloor = 4; /* Asssume elevator halts for 4 seconds in the floor requested */

            ElevatorSystemManager esm = new ElevatorSystemManager(noOfElevators, timeToCrossAFloor, haltTimeatAFloor, noOfBasement, noOfFloors);
            //esm.Start();
            esm.ReceiveUserRequest();
            //Console.WriteLine("Elevator System is starting, please wait....");
            esm.Start();
            esm.ProcessUserRequest();
            Console.Read();
        }
    }

}
