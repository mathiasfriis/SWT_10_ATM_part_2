using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATM.Logger;
using ATM.Render;

namespace ATM.Events
{
    public abstract class Event
    {
        public string _occurrenceTime { get; set; }
        public List<TrackData> _InvolvedTracks { get; set; }
        public bool _isRaised { get; set; }
        private IFileOutput _outputFile;
        private IConsoleOutput _outputConsole; 


        public Event(IFileOutput outputFile, IConsoleOutput outputConsole)
        {
            _InvolvedTracks = new List<TrackData>();
            _outputFile = outputFile;
            _outputConsole = outputConsole;
        }
        

        public virtual bool CheckIfStillValid()
        {
            if (_isRaised)
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
            _outputFile.Write(FormatData());
        }

        public void Render()
        {
            _outputConsole.Print(FormatData());
        }
    }
}
