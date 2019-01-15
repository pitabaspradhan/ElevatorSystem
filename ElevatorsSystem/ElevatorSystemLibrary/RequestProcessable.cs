using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSystemLibrary
{
    public interface IRequestProcessable
    {
        void ProcessRequest(int requestedFloor);
    }
}
