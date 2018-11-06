using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATM
{
    public class ConsoleRenderer : IRenderer
    {
        public void RenderSeperationEvent(SeperationEvent seperationEvent)
        {
            string timeOfOccurence = seperationEvent._OccurrenceTime;// + seperationEvent.OccurrenceTime.ToLongTimeString();
            string track1 = seperationEvent._InvolvedTracks[0]._Tag;
            string track2 = seperationEvent._InvolvedTracks[1]._Tag;

            Console.WriteLine("Warning: Seperation event occurred at " + timeOfOccurence + " - Involved tracks are " + track1 + " and " + track2 +".");
        }

        public void RenderTrack(TrackData trackData)
        {
            string Tag = trackData._Tag;
            double x = trackData._CurrentXcord;
            double y = trackData._CurrentYcord;
            double z = trackData._CurrentZcord;
            double horzVel = trackData._CurrentHorzVel;
            double course = trackData._CurrentCourse;

            Console.WriteLine(Tag + " - " + "(" + x + "," + y + "," + z + ")" + " - " + "Speed: " + horzVel + "m/s - Course: " + course + " degrees.");
            
        }

    }
}
