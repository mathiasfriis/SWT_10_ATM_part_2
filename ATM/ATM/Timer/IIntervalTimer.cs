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
        void StartTrackEnteredTimer(int time, TrackEnteredEvent something);

        void StartTrackLeftTimer(int time, TrackLeftEvent something);

        void TrackEnteredTimerElapsed(object sender, System.Timers.ElapsedEventArgs e);

        void TrackLeftTimerElapsed(object sender, System.Timers.ElapsedEventArgs e);

    }
}
