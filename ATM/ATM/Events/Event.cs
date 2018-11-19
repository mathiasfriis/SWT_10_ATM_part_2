using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATM.Events
{
    public abstract class Event
    {
        public string _occurrenceTime { get; set; }
        public List<TrackData> _InvolvedTracks { get; set; }
        public bool _isRaised { get; set; }

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
    }
}
