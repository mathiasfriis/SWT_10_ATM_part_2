using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATM.Events
{
    public class TrackLeftEvent : Event
    {
        public string _occurrenceTime { get; set; }
        public TrackData _involvedTrack { get; set; }
        public bool _isRaised { get; set; }

        public TrackLeftEvent(string occurrenceTime, TrackData involvedTrack, bool isRaised)
        {
            _occurrenceTime = occurrenceTime;
            _involvedTrack = involvedTrack;
            _isRaised = isRaised;

            ATM.IntervalTimer.IntervalTimer _timer = new ATM.IntervalTimer.IntervalTimer();
            _timer.StartTrackLeftTimer(5000, this);
        }

        public override string FormatData()
        {
            return "Track left airspace - Occurencetime: " + _occurrenceTime + "Involved track: " + _involvedTrack;
        }
    }
}
