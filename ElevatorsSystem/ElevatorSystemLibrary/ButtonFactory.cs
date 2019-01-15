using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSystemLibrary
{
    /* Here we Used Abstract Factory design pattern to create FloorLoby*/
    abstract class AbstractButtonFactory
    {
        public abstract IButton CreateElevatorButton();
        public abstract IButton CreateFlooLobbyrButton();

    }
    class ButtonFactory : AbstractButtonFactory
    {
        public override IButton CreateElevatorButton()
        {
            return new ElevatorButton();
        }

        public override IButton CreateFlooLobbyrButton()
        {
            return new FloorLobbyButton();
        }
        public IButton CreateButton(ButtonType buttonType)
        {
            if (buttonType == ButtonType.ElevatorButton)
            {
                return CreateElevatorButton();
            }
            else if (buttonType == ButtonType.FloorButton)
            {
                return CreateFlooLobbyrButton();
            }
            else
            {
                return null;
            }
        }
    }
}
