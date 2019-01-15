using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace ElevatorSystemLibrary
{
    public class ElevatorSystemManager
    {
        public List<Elevator> ElevatorsInHaltedState { get; set; }
        public ElevatorSystemManager(int noOfElevators, int crossTime, int haltTime, int noOfBasement, int noOfFloors )
        {
            Utility.TotalElevators = noOfElevators;
            Utility.CrossTime = crossTime;
            Utility.HaltTime = haltTime;
            if (noOfBasement != 0)
            {
                Utility.BottomFloorID = (-1) * Math.Abs(noOfBasement);
            }
            else
            {
                Utility.BottomFloorID = 0;
            }
            Utility.TopFloorID = noOfFloors;

            Utility.RequestedFloorLobbies = new Queue<int>();
            ElevatorsInHaltedState = new List<Elevator>();
            Utility.ElevatorReqFloorTimeMap = new Dictionary<Elevator, int>();
            CreateFloorLobbies();
            CreateElevators();
        }
        public void CreateFloorLobbies()
        {
            Utility.FloorLobbies = new List<AbstractFloorLobby>();
            if (Utility.BottomFloorID != 0)
            {
                /*Create Bottm floor Lobby bottom Basement */
                
                var bottomFloorLobby = FloorLobyFactory.CreateFloorLobby(FloorLobbyType.Bottom, Utility.BottomFloorID);
                Utility.FloorLobbies.Add(bottomFloorLobby);
            }
            for (int basementIndex = Utility.BottomFloorID +1; basementIndex < Utility.TopFloorID; basementIndex++)
            {
                /*Create floors lobbies between bottom floor and top floor */
                var basementLobby = FloorLobyFactory.CreateFloorLobby(FloorLobbyType.Normal, basementIndex);
            }
            /*Create Top floor Lobby  */
            
            var topFloorLobby = FloorLobyFactory.CreateFloorLobby(FloorLobbyType.Top, Utility.TopFloorID);
            Utility.FloorLobbies.Add(topFloorLobby);
        }
        public void CreateElevators()
        {
            Utility.Elevators = new List<Elevator>();
            
            for(int i=1; i<= Utility.TotalElevators; i++)
            {
                var elevator = ElevaorFactory.CreateElevator(i, Utility.CrossTime, Utility.HaltTime, Utility.BottomFloorID, Utility.TopFloorID);
                Utility.Elevators.Add(elevator);
            }
        }
        public void Start()
        {
            System.Threading.Tasks.Task.Run(()=> Utility.Elevators.ToList().ForEach(elevator => elevator.Start()));
            Console.WriteLine("Elevator System is started");
        }
        public void ReceiveUserRequest()
        {
            System.Threading.Thread.Sleep(1000);
            Console.WriteLine("Please Enter a floor number of the floor lobby. Enter valid number between {0} to {1}", Utility.BottomFloorID, Utility.TopFloorID);
            int floorNo = Convert.ToInt32(Console.ReadLine());
            while (floorNo < Utility.BottomFloorID || floorNo > Utility.TopFloorID)
            {
                Console.WriteLine("Please Enter a valid floor number between {0} to {1}", Utility.BottomFloorID, Utility.TopFloorID);
                floorNo = Convert.ToInt32(Console.ReadLine());
            }
            Utility.RequestedFloorLobbies.Enqueue(floorNo);
            int whichButtonPresssed = -1; // 0 means down button is pressed, 1 means up button is pressed, -1 means non of these button is pressed/
            if (floorNo != Utility.BottomFloorID || floorNo != Utility.TopFloorID)
            {
                Console.WriteLine("Please Enter 1 to Press Up Button and 0 to Press Down Button");
                whichButtonPresssed = Convert.ToInt32(Console.ReadLine());
                while (whichButtonPresssed != 0 && whichButtonPresssed != 1)
                {
                    Console.WriteLine("Please Enter 1 to Press Up Button and 0 to Press Down Button");
                    whichButtonPresssed = Convert.ToInt32(Console.ReadLine());
                }
            }
            else if (floorNo == Utility.TopFloorID)
            {
                Console.WriteLine("You are in Top floor, Please 0 to Press Down Button");
                whichButtonPresssed = Convert.ToInt32(Console.ReadLine());
                while (whichButtonPresssed != 0)
                {
                    Console.WriteLine("Please 0 to Press Down Button");
                    whichButtonPresssed = Convert.ToInt32(Console.ReadLine());
                }
            }
            else if (floorNo == Utility.BottomFloorID)
            {
                Console.WriteLine("You are in bottom floor, Please 1 to Press UP Button");
                whichButtonPresssed = Convert.ToInt32(Console.ReadLine());
                while (whichButtonPresssed != 1)
                {
                    Console.WriteLine("Please  Press 1 for UP Button");
                    whichButtonPresssed = Convert.ToInt32(Console.ReadLine());
                }
            }
            SetRequestedFloor(floorNo, whichButtonPresssed);
        }

        public void ProcessUserRequest()
        {
            System.Threading.Thread.Sleep(500);
            
            while (Utility.RequestedFloorLobbies.Count > 0)
            {
                int requestedFloor = Utility.RequestedFloorLobbies.Dequeue();
                //var elevatorTimeMap = ElevatorReqFloorTimeMap;
                var task = System.Threading.Tasks.Task.Run(() => Utility.ElevatorReqFloorTimeMap.Values.Min());
                var result = Utility.ElevatorReqFloorTimeMap.FirstOrDefault(x=> x.Value == task.Result);
                var taskTimer = new TaskTimer();

                if (result.Value == 0)
                {
                    taskTimer.Interval = 1000;
                }
                else
                {
                    taskTimer.Interval = result.Value * 1000 + 100 ; /* Extra 100 millisecond added for instruction execution time */
                }
                taskTimer.ProcessElevator = result.Key;
                taskTimer.RequestedFloorNo = requestedFloor;
                taskTimer.Elapsed += TaskTimer_Elapsed;
                Console.WriteLine("The nearest elevator {0} will serve the request", result.Key.ElevatorID);
                taskTimer.ProcessElevator.ProcessRequest(requestedFloor);
                taskTimer.Start();
            }
        }

        private void TaskTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var senderObject = ((TaskTimer)sender);
            
            if(senderObject.ProcessElevator.CurrentFloorNo == senderObject.RequestedFloorNo)
            {
                Console.WriteLine("The request is served by Elevator {0}", senderObject.ProcessElevator.ElevatorID);
                senderObject.Stop();
            }
            else
            {
                ElevatorsInHaltedState.ToList().Add(senderObject.ProcessElevator);
                var elevatorTimeMap = Utility.GetElevatorTimeMap(senderObject.RequestedFloorNo);
                ElevatorsInHaltedState.ToList().ForEach(item => elevatorTimeMap.Remove(item));
                if (elevatorTimeMap != null && elevatorTimeMap.Count > 0)
                {
                    var minTime = elevatorTimeMap.Values.ToList().Min();
                    var nearestElevator = elevatorTimeMap.FirstOrDefault(x => x.Value == minTime).Key;
                    senderObject.ProcessElevator = nearestElevator;
                    senderObject.ProcessElevator.ProcessRequest(senderObject.RequestedFloorNo);
                    senderObject.ProcessElevator.Start();
                    senderObject.Interval = elevatorTimeMap[nearestElevator] * 1000 + 100; /* Extra 100 millisecond added for instruction execution time */
                    Console.WriteLine("The Previous Elevator may halt somewhere.The next nearest elevator will serve the request");
                }
                
            }
        }

        private void SetRequestedFloor(int floorNo, int whichButtonPresssed)
        {
            Utility.FloorLobbies.ToList().ForEach(floorLobby =>
            {
                if (floorLobby.FloorID == floorNo)
                {
                    if (Utility.FloorLobbies[floorNo].FloorLobbyType == FloorLobbyType.Normal)
                    {
                        var x = (FloorLobby)Utility.FloorLobbies[floorNo];
                        if (whichButtonPresssed == 0)
                        {
                            x.DownButton.ISPressed = true;
                        }
                        else if (whichButtonPresssed == 1)
                        {
                            x.UpButton.ISPressed = true;
                        }
                    }
                    else if (Utility.FloorLobbies[floorNo].FloorLobbyType == FloorLobbyType.Top)
                    {
                        (Utility.FloorLobbies[floorNo] as TopFloorLobby).DownButton.ISPressed = true;

                    }
                    else if (Utility.FloorLobbies[floorNo].FloorLobbyType == FloorLobbyType.Bottom)
                    {
                        ((BottomFloorLobby)Utility.FloorLobbies[floorNo]).UpButton.ISPressed = true;

                    }
                }
            });
        }

    }
}
