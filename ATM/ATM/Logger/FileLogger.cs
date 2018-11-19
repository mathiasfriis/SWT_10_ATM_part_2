using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATM.Events;

namespace ATM.Logger
{
    public class FileLogger : ILogger
    {
        //LogFile is created in this projects "\bin" folder
        public static string startupPath = System.IO.Directory.GetCurrentDirectory();
        private string fileName = "fileLogger.txt";

        private string seperationEventActive = "Active";
        private string seperationEventInactive = "Inactive";

        private IFileOutput _fileOutput = new FileOutput();


        /*
        public void LogActiveSeparationEvent(SeperationEvent seperationEvent)
        {
            string timeOfOccurence = seperationEvent._occurrenceTime.ToString();
            TrackData track1 = seperationEvent._InvolvedTracks[0];
            TrackData track2 = seperationEvent._InvolvedTracks[1];

            //Creating instance of StreamWriter
            System.IO.StreamWriter streamWriter = System.IO.File.AppendText(startupPath + fileName);

            string lineToLog = "Timestamp: " + timeOfOccurence + "  " + "Flight 1: " + track1._Tag + " | " + "Flight 2: " + track2._Tag + " | " + "SeperationEvent status: " + seperationEventActive;
            //Perhaps it should be WriteLineAsync in order to keep up with the system
            streamWriter.WriteLine(lineToLog);

            //Closing streamWriter instance and file
            streamWriter.Close();

        }

        public void LogInactiveSeparationEvent(SeperationEvent seperationEvent)
        {
            string timeOfOccurence = seperationEvent._occurrenceTime.ToString();
            TrackData track1 = seperationEvent._InvolvedTracks[0];
            TrackData track2 = seperationEvent._InvolvedTracks[1];

            //Creating instance of StreamWriter
            System.IO.StreamWriter streamWriter = System.IO.File.AppendText(startupPath + fileName);

            string lineToLog = "Timestamp: " + timeOfOccurence + "  " + "Flight 1: " + track1._Tag + " | " + "Flight 2: " + track2._Tag + " | " + "SeperationEvent status: " + seperationEventInactive;
            //Perhaps it should be WriteLineAsync in order to keep up with the system
            streamWriter.Write(lineToLog);

            //Closing streamWriter instance and file
            streamWriter.Close();

        }
        */
        public void LogActiveEvent(Event Event)
        {
            _fileOutput.Write(Event.FormatData());
        }

        public void LogInactiveEvent(Event Event)
        {
            _fileOutput.Write(Event.FormatData());
        }

    }

}
