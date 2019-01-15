using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace ElevatorSystemLibrary
{
    public class TaskTimer : Timer
    {
        //public IDictionary<Elevator,int> ElevatorTimeStampMap { get; set; }
        public Elevator ProcessElevator { get; set; }
        public int RequestedFloorNo { get; set; }

        public TaskTimer() : base()
        {
        }
    }

}
