using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATM.Events;

namespace ATM.Logger
{
    public interface ILogger
    {

        void LogActiveSeparationEvent(SeperationEvent seperationEvent);
        void LogInactiveSeparationEvent(SeperationEvent seperationEvent);


    }
}
