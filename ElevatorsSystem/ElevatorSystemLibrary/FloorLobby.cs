using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSystemLibrary
{

    public enum FloorLobbyType
    {
        Top,
        Bottom,
        Normal
    }
    public abstract class AbstractFloorLobby : IRequestProcessable
    {
        public int  FloorID { get; set; }
        public string Description { get; set; }
        public FloorLobbyType FloorLobbyType { get; set; }
        public event ButtonPressedEventHandler ButtonPressed;

        public AbstractFloorLobby()
        {
            ButtonPressed += FloorLobby_ButtonPressed;
        }
        public void ProcessRequest(int requestedFloor)
        {
            Utility.RequestedFloorLobbies.Enqueue(requestedFloor);
        }
        private void FloorLobby_ButtonPressed(IButton btn, ButtonEventArgs btnEventArgs)
        {
            IFloorLobbyButton flButton = (IFloorLobbyButton)btn;
            ProcessRequest(flButton.FloorID);
        }
    }

    public class FloorLobby : AbstractFloorLobby
    {
        /* Dependency Injection using Constructor Injection*/
        public FloorLobby(IFloorLobbyButton upBtn, IFloorLobbyButton downBtn)
        {
            FloorLobbyType = FloorLobbyType.Normal;
            UpButton = upBtn;
            DownButton = downBtn;
        }
        public IFloorLobbyButton UpButton { get; set; }
        public IFloorLobbyButton DownButton { get; set; }
    }
    public class TopFloorLobby: AbstractFloorLobby
    {
        /* Dependency Injection using Constructor Injection*/
        public TopFloorLobby(IFloorLobbyButton btn)
        {
            FloorLobbyType = FloorLobbyType.Top;
            DownButton = btn;
        }
        public IFloorLobbyButton DownButton { get; set; }
    }

    public class BottomFloorLobby: AbstractFloorLobby
    {
        /* Dependency Injection using Constructor Injection*/
        public BottomFloorLobby(IFloorLobbyButton btn)
        {
            FloorLobbyType = FloorLobbyType.Bottom;
            UpButton = btn;
        }
        public IFloorLobbyButton UpButton { get; set; }
    }
}
