using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSystemLibrary
{

    public enum ButtonType
    {
        FloorButton,
        ElevatorButton
    }
    public interface IButton
    {
        int ButtonID { get; set; }
        string ButtonName { get; set; }
        bool ISPressed { get; set; }
        ButtonType ButtonType { get; set; }
    }

    public interface IElevatorButton : IButton
    {
        string ElevatorID { get; set; }
        string Description { get; set; }
    }

    public interface IFloorLobbyButton : IButton
    {
        int FloorID { get; set; }
        bool IsUpButton { get; set; }
        bool IsDownButton { get; set; }
    }

    class ElevatorButton : IElevatorButton
    {
        public ElevatorButton()
        {
            ButtonType = ButtonType.ElevatorButton;
        }

        public string Description { get; set; }
        public int ButtonID { get; set; }
        public string ButtonName { get; set; }
        public bool ISPressed { get; set; }
        public string ElevatorID { get; set; }
        public ButtonType ButtonType { get; set; }
    }
    class FloorLobbyButton : IFloorLobbyButton
    {
        public FloorLobbyButton()
        {
            ButtonType = ButtonType.FloorButton;
        }
        public int FloorID { get; set; }
        public bool IsUpButton { get; set; }
        public bool IsDownButton { get; set; }
        public int ButtonID { get; set; }
        public string ButtonName { get; set; }
        public bool ISPressed { get; set; }
        public ButtonType ButtonType { get; set; }
    }
}
