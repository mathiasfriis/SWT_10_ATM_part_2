﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATM.Events
{
    public class SeperationEvent : Event
    {

        private string _occurrenceTime { get; set; }
        private List<TrackData> _involvedTracks { get; set; }
        private bool _isRaised { get; set; }

        public SeperationEvent(string occurrenceTime, List<TrackData> involvedTracks, bool isRaised)
        {
            _occurrenceTime = occurrenceTime;
            _involvedTracks = involvedTracks;
            _isRaised = isRaised;
        }

        public override string FormatData()
        {
            return "Separation event - Occurencetime: " + _occurrenceTime + "Involved tracks: " + _involvedTracks[0] + ", " + _involvedTracks[1];
        }
    }
}
