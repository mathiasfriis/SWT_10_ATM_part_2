
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATM.Logger;
using ATM.Render;

namespace ATM.Events
{
    public class TrackEnteredEvent : FlightEvent

    {
        public TrackEnteredEvent(string occurrenceTime, TrackData involvedTrack, bool isRaised, IConsoleOutput outputConsole, IFileOutput outputFile) : base(outputFile, outputConsole)
        {
            base.occurrenceTime = occurrenceTime;
            InvolvedTracks.Add(involvedTrack);
            base.isRaised = isRaised;

            ATM.IntervalTimer.IntervalTimer _timer = new ATM.IntervalTimer.IntervalTimer();
            _timer.Start(5000, this);

        }

        public override string FormatData()
        {
            return "Track entered airspace - Occurencetime: " + occurrenceTime + " Involved track: " + InvolvedTracks[0].Tag;
        }

    }
}
