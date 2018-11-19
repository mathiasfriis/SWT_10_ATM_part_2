using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATM.Events;

namespace ATM.Render
{
    public class ConsoleRenderer : IRenderer
    {
        public IConsoleOutput _consoleOutput = new ConsoleOutput();

        /*
        public void RenderSeperationEvent(SeperationEvent seperationEvent)
        {
            string timeOfOccurence = seperationEvent._occurrenceTime;// + seperationEvent.OccurrenceTime.ToLongTimeString();
            string track1 = seperationEvent._InvolvedTracks[0]._Tag;
            string track2 = seperationEvent._InvolvedTracks[1]._Tag;

            string seperationEventToRender =
                $"Warning: Seperation event occurred at {timeOfOccurence} - Involved track are {track1} and {track2}.";

            _consoleOutput.Print(seperationEventToRender);
        }
        */

        public void RenderEvent(Event Event)
        {
            _consoleOutput.Print(Event.FormatData());

        }

        public string RenderTrack(TrackData trackData)
        {
            _consoleOutput.Print(trackData.FormatData());
            return trackData.FormatData();
        }
        
    }
}
