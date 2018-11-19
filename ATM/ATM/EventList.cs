using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using ATM.Events;

namespace ATM
{
    public class EventList
    {
        private List<Event> events;

        public bool CheckIfSeperationEventExistsFor(TrackData trackData1, TrackData trackData2)
        {

            if (events.Exists(x => x is SeperationEvent && x._InvolvedTracks[1]._Tag == trackData1._Tag &&
                                                     x._InvolvedTracks[0]._Tag == trackData2._Tag))
            {
                return true;
            }

            else if (events.Exists(x => x is SeperationEvent && x._InvolvedTracks[1]._Tag == trackData2._Tag &&
                                                          x._InvolvedTracks[0]._Tag == trackData1._Tag))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool checkIfTrackEnteredEventExistsFor(TrackData trackData)
        {
            if (events.Exists(x => (x is TrackEnteredEvent) && (x._InvolvedTracks[0].Equals(trackData._Tag))))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool checkIfTrackLeftEventExistsFor(TrackData trackData)
        {
            if (events.Exists(x => (x is TrackLeftEvent) && (x._InvolvedTracks[0].Equals(trackData._Tag))))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void cleanUpEvents()
        {
            //For all events, check if they are still valid. If not, remove them.
            foreach (var e in events)
            {
                if (e.CheckIfStillValid() == false)
                {
                    events.Remove(e);
                }
            }
        }
    }
}
