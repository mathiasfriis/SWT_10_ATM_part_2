using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ATM.Timer
{
    public interface ITimer
    {
        void Start(int time);

        event ElapsedEventHandler Elapsed;

        void TimerElapsed(object sender, ElapsedEventArgs e);
    }
}
