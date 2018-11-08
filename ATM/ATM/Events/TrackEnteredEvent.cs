using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATM.Events
{
    public class TrackEnteredEvent : Event
    {
        private string _occurrenceTime { get; set; }
        private List<TrackData> _involvedTracks { get; set; }
        private bool _isRaised { get; set; }

        public TrackEnteredEvent(string occurrenceTime, List<TrackData> involvedTracks, bool isRaised)
        {
            _occurrenceTime = occurrenceTime;
            _involvedTracks = involvedTracks;
            _isRaised = isRaised;
        }

        public override string FormatData()
        {
            throw new NotImplementedException();
        }

    }
}
