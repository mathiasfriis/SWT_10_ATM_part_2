using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using ATM.Events;
using ATM.Logger;

namespace ATM
{
    public class EventList
    {
        public List<FlightEvent> events;

        public EventList()
        {
            events = new List<FlightEvent>();
        }

        public bool CheckIfSeperationEventExistsFor(TrackData trackData1, TrackData trackData2)
        {
            foreach (FlightEvent e in events)
            {
                if (e is SeperationEvent)
                {
                    if ((e.InvolvedTracks[0].Tag == trackData1.Tag) && (e.InvolvedTracks[1].Tag == trackData2.Tag))
                    {
                        return true;
                    }
                    if ((e.InvolvedTracks[1].Tag == trackData1.Tag) && (e.InvolvedTracks[0].Tag == trackData2.Tag))
                    {
                        return true;
                    }
                }
            }

            return false;
            /*
            if (events == null)
            {
                return false;
            }
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
            }*/
        }

        public bool checkIfTrackEnteredEventExistsFor(TrackData trackData)
        {
            if (events.Exists(x => (x is TrackEnteredEvent) && (x.InvolvedTracks[0].Tag.Equals(trackData.Tag))))
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
            if (events.Exists(x => (x is TrackLeftEvent) && (x.InvolvedTracks[0].Tag.Equals(trackData.Tag))))
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
            List<int> indiceToRemoveAt = new List<int>();
            //For all events, check if they are still valid. If not, remove them.
            foreach (var a in events)
            {
                if (a.CheckIfStillValid() == false)
                {
                    //Add index to list of indice to remove at.
                    indiceToRemoveAt.Add(events.IndexOf(a));
                }
            }

            //Revert list, to avoid errors when iterating
            indiceToRemoveAt.Reverse();

            foreach (int i in indiceToRemoveAt)
            {
                events.RemoveAt(i);
            }
        }

        public void AddTrackEnteredEventFor(TrackData td, IFileOutput logger)
        {
            TrackEnteredEvent tee = new TrackEnteredEvent(td.TimeStamp, td, true, td.ConsoleOutput, logger);
            events.Add(tee);
            logger.Write(tee.FormatData());
        }

        public void AddTrackLeftEventFor(TrackData td, IFileOutput logger)
        {
            TrackLeftEvent tle = new TrackLeftEvent(td.TimeStamp, td, true, td.ConsoleOutput, logger);
            events.Add(tle);
            logger.Write(tle.FormatData());
        }

        public void AddSeperationEventFor(TrackData td1, TrackData td2, IFileOutput logger)
        {
            List<TrackData> tracks = new List<TrackData>()
            {
                td1,
                td2
            };
            SeperationEvent se = new SeperationEvent(td1.TimeStamp, tracks, true, td1.ConsoleOutput, logger);
            events.Add(se);
        }

        public void RenderEvents()
        {
            foreach(FlightEvent e in events)
            {
                e.Render();
            }
        }
    }
}
