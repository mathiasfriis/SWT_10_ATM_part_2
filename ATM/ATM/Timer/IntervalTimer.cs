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
        private TrackEnteredEvent _track_entered_event;
        private TrackLeftEvent _track_left_event;

        public void StartTrackEnteredTimer(int seconds, TrackEnteredEvent track_entered_event)
        {
            aTimer = new System.Timers.Timer();
            aTimer.Interval = seconds;

            _track_entered_event = track_entered_event;
            
            aTimer.Elapsed += TrackEnteredTimerElapsed;

            //Only run once
            aTimer.AutoReset = false;

            aTimer.Enabled = true;

            //Console.WriteLine("Timer has starteed");          
        }

        public void StartTrackLeftTimer(int seconds, TrackLeftEvent track_left_event)
        {
            aTimer = new System.Timers.Timer();
            aTimer.Interval = seconds;

            _track_left_event = track_left_event;


            aTimer.Elapsed += TrackLeftTimerElapsed;

            //Only run once
            aTimer.AutoReset = false;

            aTimer.Enabled = true;

            //Console.WriteLine("Timer has starteed");


        }

        public void TrackEnteredTimerElapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("State of isRaised: " + _track_entered_event._isRaised);
            _track_entered_event._isRaised = false;
            Console.WriteLine("State of isRaised: " + _track_entered_event._isRaised);
        }

        public void TrackLeftTimerElapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("State of isRaised: " + _track_left_event._isRaised);
            _track_left_event._isRaised = false;
            Console.WriteLine("State of isRaised: " + _track_left_event._isRaised);
        }
    }
}
