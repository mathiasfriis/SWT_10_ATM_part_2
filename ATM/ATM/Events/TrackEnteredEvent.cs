using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATM.Logger;
using ATM.Render;

namespace ATM.Events
{
    public class TrackEnteredEvent : Event
    {   public TrackData _involvedTrack { get; set; }
     
        public TrackEnteredEvent(string occurrenceTime, TrackData involvedTrack, bool isRaised, IRenderer renderer, ILogger logger) : base(renderer, logger)
        {
            _occurrenceTime = occurrenceTime;
            _involvedTrack = involvedTrack;
            _isRaised = isRaised;

            ATM.IntervalTimer.IntervalTimer _timer = new ATM.IntervalTimer.IntervalTimer();
            _timer.Start(5000, this);

        }

        public override string FormatData()
        {
            return "Track entered airspace - Occurencetime: " + _occurrenceTime + "Involved track: " + _involvedTrack;
        }

    }
}
