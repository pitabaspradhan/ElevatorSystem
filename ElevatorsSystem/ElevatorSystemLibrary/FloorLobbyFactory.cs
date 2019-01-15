namespace ElevatorSystemLibrary
{
    /* Here we Used Factory method design pattern to create FloorLoby*/
    public class FloorLobyFactory
    {
        public static AbstractFloorLobby CreateFloorLobby(FloorLobbyType floorLobbyType, int floorID)
        {
            if (floorLobbyType == FloorLobbyType.Top)
            {
                IFloorLobbyButton btn = GetDownButton(floorID);
                /* Dependency Injection using Constructor Injection*/
                return new TopFloorLobby(btn);
            }
            else if (floorLobbyType == FloorLobbyType.Bottom)
            {
                IFloorLobbyButton btn = GetUpButton(floorID);
                /* Dependency Injection using Constructor Injection*/
                return new BottomFloorLobby(btn);
            }
            else
            {
                IFloorLobbyButton upBtn = GetUpButton(floorID);
                IFloorLobbyButton downBtn = GetDownButton(floorID);
                /* Dependency Injection using Constructor Injection*/
                return new FloorLobby(upBtn, downBtn);
            }
        }
        private static IFloorLobbyButton GetUpButton(int floorID)
        {
            ButtonFactory btnFactory = new ButtonFactory();
            IFloorLobbyButton btn = btnFactory.CreateButton(ButtonType.FloorButton) as IFloorLobbyButton;
            btn.ButtonID = 1;
            btn.FloorID = floorID;
            btn.IsUpButton = true;
            btn.ButtonName = string.Format("Up Button at Floor {0}", floorID);
            return btn;
        }
        private static IFloorLobbyButton GetDownButton(int floorID)
        {
            ButtonFactory btnFactory = new ButtonFactory();
            IFloorLobbyButton btn = btnFactory.CreateButton(ButtonType.FloorButton) as IFloorLobbyButton;
            btn.ButtonID = 0;
            btn.FloorID = floorID;
            btn.IsDownButton = true;
            btn.ButtonName = string.Format("Down Button at Floor {0}", floorID);
            return btn;
        }

    }
}
