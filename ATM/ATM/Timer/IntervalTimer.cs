using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using ATM.Events;

namespace ATM.IntervalTimer
{
    class IntervalTimer : IIntervalTimer
    {
        private static Timer aTimer;
        private Event _trackEvent;

        public void Start(int seconds, Event trackEvent)
        {
            aTimer = new System.Timers.Timer();
            aTimer.Interval = seconds;

            _trackEvent = trackEvent;
            
            aTimer.Elapsed += TimerElapsed;

            //Only run once
            aTimer.AutoReset = false;

            aTimer.Enabled = true;

            //Console.WriteLine("Timer has starteed");          
        }

        public void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            //Console.WriteLine("State of isRaised: " + _trackEvent._isRaised);
            _trackEvent._isRaised = false;
            //Console.WriteLine("State of isRaised: " + _trackEvent._isRaised);
        }
    }
}
