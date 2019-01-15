using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSystemLibrary
{
    /* An interface that defines actions that the receiver can perform */
    public interface IMovable
    {
        void Start();
        void MoveUp();
        void MoveDown();
        void Halt();
        void Stop();

    }
}
