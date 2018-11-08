using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATM.Events
{
    public class TrackLeftEvent : Event
    {
        private string _occurrenceTime { get; set; }
        private TrackData _involvedTrack { get; set; }
        private bool _isRaised { get; set; }

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
