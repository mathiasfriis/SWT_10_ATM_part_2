using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATM.Logger;
using ATM.Render;

namespace ATM.Events
{
    public class SeperationEvent : FlightEvent
    {
        public SeperationEvent(string occurrenceTime, List<TrackData> involvedTracks, bool isRaised, IConsoleOutput outputConsole, IFileOutput outputFile) : base(outputFile, outputConsole)

        {
            base.occurrenceTime = occurrenceTime;
            InvolvedTracks = involvedTracks;
            base.isRaised = isRaised;

        }

        public override string FormatData()
        {
            return "Separation event - Occurencetime: " + occurrenceTime + " Involved tracks: " + InvolvedTracks[0].Tag + ", " + InvolvedTracks[1].Tag;
        }
    }
}
