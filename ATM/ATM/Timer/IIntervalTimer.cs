using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using ATM.Events;

namespace ATM.IntervalTimer
{
    public interface IIntervalTimer
    {
        void Start(int time, FlightEvent trackEvent);

        void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e);

        
    }
}
