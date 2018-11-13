using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ATM.IntervalTimer
{
    class IntervalTimer : IIntervalTimer
    {
        private static Timer aTimer;

        public void Start(int seconds)
        {
            aTimer = new System.Timers.Timer();
            aTimer.Interval = seconds;

            aTimer.Elapsed += TimerElapsed;

            //Only run once
            aTimer.AutoReset = false;

            aTimer.Enabled = true;

            Console.WriteLine("Timer has starteed");
        }

        
        public void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("Reached TimerElapsed state");
        }
    }
}
