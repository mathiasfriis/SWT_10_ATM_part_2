using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATM.Events
{
    public abstract class Event
    {
        private string _occurrenceTime { get; set; }
        private List<TrackData> _InvolvedTracks { get; set; }
        private bool _isRaised { get; set; }

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
