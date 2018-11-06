using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATM
{
    public class TrackData
    {

        private string _tag, _timeStamp;
        private double _currentXcord, _currentYcord, _currentZcord, _currentHorzVel, _currentCourse;

        public TrackData(string tag, double currentXcord, double currentYcord, double currentZcord, 
                         string timeStamp, double currentHorzVel, double currentCourse)
        {
            _tag = tag;
            _currentXcord = currentXcord;
            _currentYcord = currentYcord;
            _currentZcord = currentZcord;
            _timeStamp = timeStamp;
            _currentHorzVel = currentHorzVel;
            _currentCourse = currentCourse;
        }


       
        public String _Tag { get { return _tag; } set { _tag = value; } }
        public double _CurrentXcord { get { return _currentXcord; } set { _currentXcord = value; } }
        public double _CurrentYcord { get { return _currentYcord; } set { _currentYcord = value; } }
        public double _CurrentZcord { get { return _currentZcord; } set { _currentZcord = value; } }
        public string _TimeStamp { get { return _timeStamp; } set { _timeStamp = value; } }
        public double _CurrentHorzVel { get { return _currentHorzVel; } set { _currentHorzVel = value; } }
        public double _CurrentCourse { get { return _currentCourse; } set { _currentCourse = value; } }


    }
}
