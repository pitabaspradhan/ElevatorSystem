using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ElevatorSystemLibrary
{
    public class ElevaorFactory
    {
        public static Elevator CreateElevator(int index,int crossTime, int haltTime,int bottomFloor, int topFloor)
        {
            string id = string.Format("E{0}", index);
            Timer t = new Timer();
            IElevatorButton[] buttons = GetElevatorButtons(bottomFloor, topFloor);
            IDictionary<int, bool> floorsMap = GetFloorsStopMap(bottomFloor, topFloor);
            Queue<int> requestedQueue = new Queue<int>();
            var floor = floorsMap.FirstOrDefault(item => item.Value == true).Key;
            requestedQueue.Enqueue(floor);
            return new Elevator(id,t,buttons,floorsMap,crossTime,haltTime,requestedQueue);
        }

        private static IElevatorButton[] GetElevatorButtons(int bottomFloor, int topFloor)
        {
            IElevatorButton[] elevatorButtons = new ElevatorButton[topFloor+ Math.Abs(bottomFloor)+1];
            int index = 0;
            ButtonFactory btnFactory = new ButtonFactory();
            for (int i = bottomFloor; i<=topFloor; i++)
            {
                elevatorButtons[index++] = GetElevatorButton(btnFactory, i);
            }
            return elevatorButtons;
        }
        private static IElevatorButton GetElevatorButton(ButtonFactory btnFactory, int i)
        {
            IElevatorButton btn = (IElevatorButton)btnFactory.CreateButton(ButtonType.ElevatorButton);
            btn.ButtonID = i;
            btn.ButtonName = i.ToString();
            btn.Description = string.Format("Press this button to go to Floor No.{0}", btn.ButtonID);
            return btn;
        }

        /* Creating list of floor where elevator needs to be stopped. 
         * Here we are slecting one floor randomly at beginning where Elevator needs to stop.
         */
        private static IDictionary<int,bool> GetFloorsStopMap(int bottomFloor, int topFloor)
        {
            IDictionary<int, bool> floorsMap = new Dictionary<int, bool>();
            for (int i = bottomFloor; i <= topFloor; i++)
            {
                floorsMap.Add(i, false);
            }
            /* slect a random floor where Elevator needs to stop */
            int floorNeedsToStop = Utility.RandomNumber(bottomFloor, topFloor);
            floorsMap[floorNeedsToStop] = true;
            return floorsMap;
        }
    }
}
