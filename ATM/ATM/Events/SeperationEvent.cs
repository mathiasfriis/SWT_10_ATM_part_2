using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATM.Events
{
    public class SeperationEvent : Event
    {

        public string _occurrenceTime { get; set; }
        public List<TrackData> _InvolvedTracks { get; set; }
        public bool _isRaised { get; set; }

        public SeperationEvent(string occurrenceTime, List<TrackData> involvedTracks, bool isRaised)
        {
            _occurrenceTime = occurrenceTime;
            _InvolvedTracks = involvedTracks;
            _isRaised = isRaised;
        }

        public override string FormatData()
        {
            return "Separation event - Occurencetime: " + _occurrenceTime + "Involved tracks: " + _InvolvedTracks[0] + ", " + _InvolvedTracks[1];
        }
    }
}
