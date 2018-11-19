using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATM.Events;

namespace ATM
{
    class EventList
    {
        private List<Event> events;

        public bool CheckIfSeperationEventExistsFor(TrackData trackData1, TrackData trackData2)
        {

            if (events.Exists(x => x._InvolvedTracks[1]._Tag == trackData1._Tag &&
                                                     x._InvolvedTracks[0]._Tag == trackData2._Tag))
            {
                return true;
            }

            else if (events.Exists(x => x._InvolvedTracks[1]._Tag == trackData2._Tag &&
                                                          x._InvolvedTracks[0]._Tag == trackData1._Tag))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
