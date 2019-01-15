using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using static ElevatorSystemLibrary.Utility;

namespace ElevatorSystemLibrary
{
    public class Elevator : IMovable, IRequestProcessable
    {
        public string ElevatorID { get; set; }
        public int CurrentFloorNo { get; set; }
        public int RequestedFloor { get; set; }
        public Queue<int> RequestedFloorQueue { get; set; }
        public Direction ElevatorDirection { get; set; }
        public int CrossTime { get; set; } // Time to cross one floor in seconds.
        public int HaltTime { get; set; } // Time to halt in a floor in seconds.
        public IDictionary<int,bool> FloorsStopMap { get; set; }
        public IElevatorButton[] ElevatorButtons { get; set; }
        public System.Timers.Timer Timer { get; set ; }
        public event ButtonPressedEventHandler ButtonPressed;


        public Elevator(string id, System.Timers.Timer t, IElevatorButton[]buttons,IDictionary<int,bool>floorsStopMap, int crossTime, int haltTime, Queue<int>requestQueue)
        {
            ElevatorID = id;
            Timer =  t;
            CrossTime = crossTime;
            HaltTime = haltTime;
            ElevatorButtons = buttons;
            ElevatorButtons.ToList().ForEach(btn => btn.ElevatorID = ElevatorID);
            FloorsStopMap = floorsStopMap;
            Thread.Sleep(50);
            CurrentFloorNo = Utility.RandomNumber(Utility.BottomFloorID, Utility.TopFloorID);
            RequestedFloorQueue = requestQueue;
            ElevatorDirection = Utility.GetElevatorDirection(CurrentFloorNo, RequestedFloor);
            this.ButtonPressed += new ButtonPressedEventHandler(OnButtonPressed);
        }
        public void Start()
        {
            if (Timer.Enabled == false)
            {
                Timer.Interval = CrossTime * 1000; /* converting from seconds to milliseconds */
                Timer.Elapsed += Timer_Elapsed;
                Timer.Enabled = true;
                Timer.Start();
                System.Threading.Tasks.Task.Run(() =>
                Console.WriteLine("The Elevator {0} starts from the floor number  {1}", ElevatorID, CurrentFloorNo));
                RequestedFloor = RequestedFloorQueue.Dequeue();
                ElevatorDirection = Utility.GetElevatorDirection(CurrentFloorNo, RequestedFloor);
                int t = Utility.FindTimeTaken(this, Utility.RequestedFloorLobbies.Peek());
                Utility.ElevatorReqFloorTimeMap.Add(this, t);
            }
        }
        public void MoveUp()
        {
            CurrentFloorNo ++;
            HaltORCrossTheFloor();
            Console.WriteLine("The Elevator {0} is moving Up", ElevatorID);
            Console.WriteLine("The Current Floor is {0} ", CurrentFloorNo);
        }
        public void MoveDown()
        {
            CurrentFloorNo--;
            HaltORCrossTheFloor();
            Console.WriteLine("The Elevator {0} is moving Down", ElevatorID);
            Console.WriteLine("The Current Floor is {0} ", CurrentFloorNo);
        }
        public void Halt()
        {
            Timer.Interval = (HaltTime + CrossTime)*1000;
            FloorsStopMap[CurrentFloorNo] = false;
          
            var extraHalt = Utility.RandomNumber(0, 1); /* Randomly select extra halt, 0 means no extra halt, 1 means halt for extra time */
            if (this.ElevatorID == "E1") extraHalt = 1;
            if (extraHalt == 1)
            {
                var extraHaltTime = Utility.RandomNumber(10, 20); /* how many seconds it halts */
                Timer.Interval = (HaltTime + CrossTime + extraHaltTime) * 1000;
                Console.WriteLine("The Elevator {0} is halt on the floor number  {1} for {2} seconds", ElevatorID, CurrentFloorNo, Timer.Interval/1000);
            }
            else
            {
                Timer.Interval = (HaltTime + CrossTime) * 1000;
                Console.WriteLine("The Elevator {0} is halt on the floor number  {1} for {2} seconds", ElevatorID, CurrentFloorNo, HaltTime);
            }
            var isButtonPressed = Utility.RandomNumber(0, 1); /* Does the person wants to press any button inside elevtor , 0 means no, 1 means yes */
            if(isButtonPressed == 1)
            {
                var pressedButtonNo = Utility.RandomNumber(Utility.BottomFloorID, Utility.TopFloorID);
                var button = ElevatorButtons.FirstOrDefault(btn => btn.ButtonID == pressedButtonNo);
                button.ISPressed = true;
                if(ButtonPressed != null)
                {
                    ButtonPressed(button, new ButtonEventArgs(pressedButtonNo));
                }
            }
            this.Stop();
        }
        public void Stop()
        {
            var noStopsatAnyFLoor = FloorsStopMap.All(floorstopKeyValue => floorstopKeyValue.Value == false);
            if(noStopsatAnyFLoor == true)
            {
                Console.WriteLine("The Elevator {0} is stopped at the floor  {1}", ElevatorID, CurrentFloorNo);
                ElevatorDirection = Direction.NoDirection;
                Timer.Enabled = false;
                Timer.Stop();
            }
        }
        public void ProcessRequest(int requestedFloor)
        {
            RequestedFloorQueue.Enqueue(requestedFloor);
            FloorsStopMap[requestedFloor] = true;
            ElevatorButtons.FirstOrDefault(btn => btn.ButtonID == requestedFloor);
            if (!Timer.Enabled)
            {
                Start();
            }
        }
        /// <summary>
        /// If there theres is a stop at the current floor then stops at current floor for an interval of halt time else cross the floor;
        /// </summary>
        private void HaltORCrossTheFloor()
        {
            if (FloorsStopMap[CurrentFloorNo] == true)
            {
                Halt();
            }
            else
            {
                Timer.Interval = CrossTime * 1000;
            }
        }
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var noStopsatAnyFLoor = FloorsStopMap.All(floorstopKeyValue => floorstopKeyValue.Value == false);
            if (noStopsatAnyFLoor == true)
            {
                Stop();
            }
            else
            {
                if (ElevatorDirection == Direction.Up)
                {
                    if (CurrentFloorNo == Utility.TopFloorID)
                    {
                        Stop();
                    }
                    else
                    {
                        MoveUp();
                    }
                }
                else if (ElevatorDirection == Direction.Down)
                {
                    if (CurrentFloorNo == Utility.BottomFloorID)
                    {
                        Stop();
                    }
                    else
                    {
                        MoveDown();
                    }
                }
            }
        }
        private void OnButtonPressed(IButton sender, ButtonEventArgs e)
        {
            ProcessRequest(e.ButtonID);
        }
    }
}

