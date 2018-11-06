using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using TransponderReceiver;

namespace ATM
{
    public class ATMclass : IObserver
    {
        double MIN_X_DISTANCE = 5000;
        double MIN_Y_DISTANCE = 5000;
        double MIN_Z_DISTANCE = 300;

        private ILogger _logger;
        private IRenderer _renderer;
        private IAirspace _airspace;
        //private ITransponderReceiver _transponderReceiver;

        public List<TrackData> _currentTracks { get; }
        public List<SeperationEvent> _currentSeperationEvents { get; }

        

        public ATMclass(ILogger logger, IRenderer renderer, IAirspace airspace)
        {
            _logger = logger;
            _renderer = renderer;
            //_transponderReceiver = transponderReceiver;
            _airspace = airspace;
            _currentSeperationEvents = new List<SeperationEvent>();
            _currentTracks = new List<TrackData>();
        }

        public void HandleNewTrackData(TrackData trackdata)
        {
            //Check if no track data with given tag exists.
            if (_currentTracks.Exists(x => x._Tag == trackdata._Tag) == false)
            {
                // Add the new track if coordinates are inside the given boundaries of the airspace.
                if (_airspace.CheckIfInMonitoredArea(trackdata._CurrentXcord, trackdata._CurrentYcord,
                    trackdata._CurrentZcord))
                {
                    AddTrack(trackdata);
                }
            }
            else
            {
                // Update trackdata
                TrackData trackToEdit = _currentTracks.Find(x => x._Tag == trackdata._Tag);
                trackToEdit._CurrentXcord = trackdata._CurrentXcord;
                trackToEdit._CurrentYcord = trackdata._CurrentYcord;
                trackToEdit._CurrentZcord = trackdata._CurrentZcord;
                trackToEdit._CurrentCourse = trackdata._CurrentCourse;
                trackToEdit._CurrentHorzVel = trackToEdit._CurrentHorzVel;

                // Remove tracks if out of airspace

                if (!(_airspace.CheckIfInMonitoredArea(trackToEdit._CurrentXcord, trackToEdit._CurrentYcord,
                    trackToEdit._CurrentZcord)))
                {
                    RemoveTrack(trackToEdit._Tag);
                }
                // Check for potential seperation events
                CheckForSeperationEvents(trackToEdit);
            }

            // Check for potential seperation events
            CheckForSeperationEvents(trackdata);


            // Remove separations event after update
            RemoveSeparationEvents();

            // Render updated tracks to console 
            RenderTracks();

            // Render seperation events
            RenderSeperationEvents();
        }

        public void AddTrack(TrackData trackData)
        {
            //Check if TrackData with given tag already exists.
            if(_currentTracks.Exists(x => x._Tag == trackData._Tag))
            {
                //Find index of existing data.
                int index = _currentTracks.FindIndex(x => x._Tag == trackData._Tag);
                //replace existing data with new data.
                _currentTracks[index] = trackData;
            }
            else
            {
                //Add trackData.
                _currentTracks.Add(trackData);
            }
            
        }

        public void RemoveTrack(string tag)
        {
            int index = _currentTracks.FindIndex(x => x._Tag.Equals(tag));
            _currentTracks.RemoveAt(index);
        }

        public void CheckForSeperationEvents(TrackData trackData)
        {
            foreach (var track in _currentTracks)
            {
                if (trackData != null && track._Tag != trackData._Tag)
                {
                    CheckForSeperationEvent(trackData, track);
                }   
            }
        }

        public bool CheckForSeperationEvent(TrackData trackData1, TrackData trackData2)
        {
            //Check if both tracks are the same
            if(trackData1._Tag==trackData2._Tag)
            {
                throw new Exception("Provided TrackDatas have the same Tag");
            }
            else
            {
                //Check if conditions are met with the given TrackData-objects
                if (Math.Abs(trackData1._CurrentXcord - trackData2._CurrentXcord) < MIN_X_DISTANCE &&
                    Math.Abs(trackData1._CurrentYcord - trackData2._CurrentYcord) < MIN_Y_DISTANCE &&
                    Math.Abs(trackData1._CurrentZcord - trackData2._CurrentZcord) < MIN_Z_DISTANCE)
                {
                    //If seperation event does not exist yet, create it and add it to currentSeperationEvents
                    //Check if separation event already exists
                    if (!CheckIfSeperationEventExistsFor(trackData1, trackData2))
                    {
                        // Add new separation event 
                        //string time = DateTime.Now.ToString();
                        string time = trackData1._TimeStamp;
                        List<TrackData> trackDataInSeperationEvent = new List<TrackData>();
                        trackDataInSeperationEvent.Add(trackData1);
                        trackDataInSeperationEvent.Add(trackData2);

                        SeperationEvent SeperationEvent = new SeperationEvent(time, trackDataInSeperationEvent, true);
                        _currentSeperationEvents.Add(SeperationEvent);
                        _logger.LogActiveSeparationEvent(SeperationEvent);
                    }

                    return true;
                }

                else
                    return false;
            }
        }

        public bool CheckIfSeperationEventExistsFor(TrackData trackData1, TrackData trackData2)
        {

            if(_currentSeperationEvents.Exists(x => x._InvolvedTracks[1]._Tag == trackData1._Tag && 
                                                    x._InvolvedTracks[0]._Tag == trackData2._Tag))
            {
                return true;
            }

            else if(_currentSeperationEvents.Exists(x => x._InvolvedTracks[1]._Tag == trackData2._Tag &&
                                                         x._InvolvedTracks[0]._Tag == trackData1._Tag))
            {
                return true;
            }
            else
            {
                return false;
            }
                                                                                                                                          
        }


        public void RenderSeperationEvents()
        {
            foreach (var seperationEvent in _currentSeperationEvents)
            {
                _renderer.RenderSeperationEvent(seperationEvent);
            }
            Console.WriteLine("Number of separation events: " + _currentSeperationEvents.Count);
        }

        public void RenderTracks()
        {
            Console.Clear();

            foreach (var trackData in _currentTracks)
            {
                _renderer.RenderTrack(trackData);
            }

        }

        public void RemoveSeparationEvents()
        {
            //Log if conditions for seperation event are no longer met.
            foreach (var separationEvent in _currentSeperationEvents)
            {
                if (!(Math.Abs(separationEvent._InvolvedTracks[0]._CurrentXcord -
                             separationEvent._InvolvedTracks[1]._CurrentXcord) < MIN_X_DISTANCE &&
                    Math.Abs(separationEvent._InvolvedTracks[0]._CurrentYcord -
                             separationEvent._InvolvedTracks[1]._CurrentYcord) < MIN_Y_DISTANCE &&
                    Math.Abs(separationEvent._InvolvedTracks[0]._CurrentZcord -
                             separationEvent._InvolvedTracks[1]._CurrentZcord) < MIN_Z_DISTANCE))
                {
                    _logger.LogInactiveSeparationEvent(separationEvent);
                }
            }

            //After logging, remove the given elements.
            _currentSeperationEvents.RemoveAll(x => Math.Abs(x._InvolvedTracks[0]._CurrentXcord -
                             x._InvolvedTracks[1]._CurrentXcord) < MIN_X_DISTANCE &&
                    Math.Abs(x._InvolvedTracks[0]._CurrentYcord -
                             x._InvolvedTracks[1]._CurrentYcord) < MIN_Y_DISTANCE &&
                    Math.Abs(x._InvolvedTracks[0]._CurrentZcord -
                             x._InvolvedTracks[1]._CurrentZcord) < MIN_Z_DISTANCE);
        }

        public void Update(TrackData trackdata)
        {
            HandleNewTrackData(trackdata);
        }
        
    }
}
