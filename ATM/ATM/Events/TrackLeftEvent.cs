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
        }

        public override string FormatData()
        {
            return "Track left airspace - Occurencetime: " + _occurrenceTime + "Involved track: " + _involvedTrack;
        }
    }
}
