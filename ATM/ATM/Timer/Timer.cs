using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ATM.Timer
{
    class Timer : ITimer
    {
        private System.Timers.Timer interval_timer;

        public event ElapsedEventHandler Elapsed;

        public void Start(int time)
        {
            interval_timer = new System.Timers.Timer(time);

            //Only run once
            interval_timer.AutoReset = true;

            interval_timer.Elapsed += TimerElapsed;

            interval_timer.Start();
        }

        public void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            // Runs after time_argument from Start function has elapsed
        }
    }
}
