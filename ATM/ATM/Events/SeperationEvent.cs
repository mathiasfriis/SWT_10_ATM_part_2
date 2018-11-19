using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATM.Logger;
using ATM.Render;

namespace ATM.Events
{
    public class SeperationEvent : Event
    {
        public SeperationEvent(string occurrenceTime, List<TrackData> involvedTracks, bool isRaised, IRenderer renderer, ILogger logger) : base(renderer, logger)
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
