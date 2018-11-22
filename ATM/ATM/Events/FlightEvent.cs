using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATM.Logger;
using ATM.Render;

namespace ATM.Events
{
    public abstract class FlightEvent
    {
        public string occurrenceTime { get; set; }
        public List<TrackData> InvolvedTracks { get; set; }
        public bool isRaised { get; set; }
        private IFileOutput outputFile;
        private IConsoleOutput outputConsole; 


        public FlightEvent(IFileOutput outputFile, IConsoleOutput outputConsole)
        {
            InvolvedTracks = new List<TrackData>();
            this.outputFile = outputFile;
            this.outputConsole = outputConsole;
        }
        

        public virtual bool CheckIfStillValid()
        {
            if (isRaised)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public abstract string FormatData();

        public void Log()
        {
            outputFile.Write(FormatData());
        }

        public void Render()
        {
            outputConsole.Print(FormatData());
        }
    }
}
