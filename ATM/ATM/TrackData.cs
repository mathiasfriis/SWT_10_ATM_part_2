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


        public String _Tag { get { return _tag; } set { _tag = value; } }
        public double _CurrentXcord { get { return _currentXcord; } set { _currentXcord = value; } }
        public double _CurrentYcord { get { return _currentYcord; } set { _currentYcord = value; } }
        public double _CurrentZcord { get { return _currentZcord; } set { _currentZcord = value; } }
        public string _TimeStamp { get { return _timeStamp; } set { _timeStamp = value; } }
        public double _CurrentHorzVel { get { return _currentHorzVel; } set { _currentHorzVel = value; } }
        public double _CurrentCourse { get { return _currentCourse; } set { _currentCourse = value; } }

        public IConsoleOutput _consoleOutput { get { return _output;} }

        public void Render()
        {
            _output.Print(FormatData());

        }

        public string FormatData()
        {
            string Tag = this._Tag;
            double x = this._CurrentXcord;
            double y = this._CurrentYcord;
            double z = this._CurrentZcord;
            double horzVel = this._CurrentHorzVel;
            double course = this._CurrentCourse;

            string trackInfoToRender = $"{Tag} - ( {x}, {y}, {z}) - Speed: {horzVel} m/s - Course: {course} degrees";

            return trackInfoToRender;
        }

    }
}
