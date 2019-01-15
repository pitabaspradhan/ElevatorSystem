using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSystemLibrary
{
    public delegate void ButtonPressedEventHandler(IButton btn, ButtonEventArgs btnEventArgs);
    public class ButtonEventArgs : EventArgs
    {
        public int ButtonID { get; set; }
        public ButtonEventArgs(int id)
        {
            ButtonID = id;
        }
    }
}
