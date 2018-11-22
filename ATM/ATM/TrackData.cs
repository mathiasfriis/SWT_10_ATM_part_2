using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATM.Logger;
using ATM.Render;

namespace ATM
{
    public class TrackData
    {

        private string _tag, _timeStamp;
        private double _currentXcord, _currentYcord, _currentZcord, _currentHorzVel, _currentCourse;
        private IConsoleOutput _output;

        public TrackData(string tag, double currentXcord, double currentYcord, double currentZcord, 
                         string timeStamp, double currentHorzVel, double currentCourse, IConsoleOutput output)
        {
            _tag = tag;
            _currentXcord = currentXcord;
            _currentYcord = currentYcord;
            _currentZcord = currentZcord;
            _timeStamp = timeStamp;
            _currentHorzVel = 0;
            _currentCourse = 0;
            _output = output;

        }


        public string Tag { get { return _tag; } set { _tag = value; } }
        public double CurrentXcord { get { return _currentXcord; } set { _currentXcord = value; } }
        public double CurrentYcord { get { return _currentYcord; } set { _currentYcord = value; } }
        public double CurrentZcord { get { return _currentZcord; } set { _currentZcord = value; } }
        public string TimeStamp { get { return _timeStamp; } set { _timeStamp = value; } }
        public double CurrentHorzVel { get { return _currentHorzVel; } set { _currentHorzVel = value; } }
        public double CurrentCourse { get { return _currentCourse; } set { _currentCourse = value; } }

        public IConsoleOutput ConsoleOutput { get { return _output;} }

        public void Render()
        {
            _output.Print(FormatData());

        }

        public string FormatData()
        {
            string Tag = this.Tag;
            double x = this.CurrentXcord;
            double y = this.CurrentYcord;
            double z = this.CurrentZcord;
            double horzVel = this.CurrentHorzVel;
            double course = this.CurrentCourse;

            string trackInfoToRender = $"{Tag} - ( {x}, {y}, {z}) - Speed: {horzVel} m/s - Course: {course} degrees";

            return trackInfoToRender;
        }

    }
}
