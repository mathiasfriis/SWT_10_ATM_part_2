﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using TransponderReceiver;
using ATM.Render;
using ATM.Logger;
using ATM.Events;
using System.Timers;
using ATM;
using ATM.IntervalTimer;

namespace ATM
{
    public class ATMclass : IObserver
    {
        double MIN_X_DISTANCE = 5000;
        double MIN_Y_DISTANCE = 5000;
        double MIN_Z_DISTANCE = 300;


        public IConsoleOutput _outputConsole;
        public IFileOutput _outputFile;

        private IAirspace _airspace;
        private ITransponderReceiver _transponderReceiver;

        public List<TrackData> _currentTracks { get; }
        public EventList _currentEvents { get; }

        private static Timer Timer;

        public ATMclass(IConsoleOutput outputConsole, IFileOutput outputFile, IAirspace airspace, ITransponderReceiver transponderReceiver)

        {
            _outputConsole = outputConsole;
            _outputFile = outputFile;
            _transponderReceiver = transponderReceiver;
            _airspace = airspace;
            _currentEvents = new EventList();
            _currentTracks = new List<TrackData>();

            Timer = new System.Timers.Timer();
            Timer.Interval = 0.05;
            Timer.Elapsed += TimerElapsed;
            Timer.AutoReset = true;
            Timer.Enabled = true;

        }

        public void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            _outputConsole.Clear();
            RenderEvents();
            RenderTracks();
            _currentEvents.cleanUpEvents();
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
                    //string time = trackdata._TimeStamp;
                    //TrackEnteredEvent TrackEnteredEvent = new TrackEnteredEvent(time, trackdata, true, _outputConsole, _outputFile);
                    _currentEvents.AddTrackEnteredEventFor(trackdata, _outputFile);
                    
                }
            }
            else
            {
                //Update trackdata
                UpdateTrackData(trackdata);

                // Remove tracks if out of airspace
                CheckIfTrackdataIsStillInAirspace(trackdata);

                // Check for potential seperation events
                CheckForSeperationEvents(trackdata);
            }

            //Remove all events that are not relevant anymore
            _currentEvents.cleanUpEvents();

            // Check for potential seperation events
            CheckForSeperationEvents(trackdata);

            // Remove separations event after update
            UpdateSeperationEventStatus();

            // Clear console 
            _outputConsole.Clear();

            // Render updated tracks to console 
            RenderTracks();

            // Render seperation events
            RenderEvents();

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
                    if(CheckForSeperationEventConditions(trackData, track)==true)
                    {
                        //If seperation event does not exist yet, create it and add it to currentSeperationEvents
                        //Check if separation event already exists
                        if (!_currentEvents.CheckIfSeperationEventExistsFor(trackData, track))
                        {
                            // Add new separation event 
                            //string time = DateTime.Now.ToString();
                            string time = trackData._TimeStamp;
                            List<TrackData> trackDataInSeperationEvent = new List<TrackData>();
                            trackDataInSeperationEvent.Add(trackData);
                            trackDataInSeperationEvent.Add(track);

                            //_currentEvents.Add(SeperationEvent);
                            _currentEvents.AddSeperationEventFor(trackData, track, _outputFile);

                            //Create event to log
                            SeperationEvent SeperationEvent = new SeperationEvent(time, trackDataInSeperationEvent, true, _outputConsole, _outputFile);
                            SeperationEvent.Log();
                        }
                    }
                }   
            }
        }

        public bool CheckForSeperationEventConditions(TrackData trackData1, TrackData trackData2)
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
                    return true;
                }

                else
                    return false;
            }
        }

        public void RenderEvents()
        {

            _currentEvents.RenderEvents();
            Console.WriteLine("Number of separation events: " + _currentEvents.events.OfType<SeperationEvent>().Count());
        }

        public void RenderTracks()
        {
            //Console.Clear();

            foreach (var trackData in _currentTracks)
            {
                trackData.Render();
            }

        }

        public void UpdateSeperationEventStatus()
        {
            //Log if conditions for seperation event are no longer met.
            foreach (var e in _currentEvents.events)
            {
                if (e is SeperationEvent)
                {
                    string tag0 = e._InvolvedTracks[0]._Tag;
                    string tag1 = e._InvolvedTracks[1]._Tag;

                    TrackData track0;
                    TrackData track1;

                    //get updated track data
                    try
                    {
                        track0 = _currentTracks.Find(x => x._Tag == tag0);
                        track1 = _currentTracks.Find(x => x._Tag == tag1);
                        //if the updated track data no longer matches conditions for SeperationEvent, set the isRaise-attribute to false
                        if (CheckForSeperationEventConditions(track0, track1) == false)
                        {
                            //Mark that seperation event is no longer active - It will now be removed at next "cleanUp"
                            e._isRaised = false;
                        }
                    }
                    catch
                    {
                        //Mark that seperation event is no longer active - It will now be removed at next "cleanUp"
                        e._isRaised = false;
                    }
                    

                    
                }
                
            }

            //After logging, remove the given elements.
            /* Temp udkommentering indtil Mathias fikser det med nye liste af events i ny klasse
            /*
            _currentEvents.RemoveAll(x => Math.Abs(x._InvolvedTracks[0]._CurrentXcord -
                             x._InvolvedTracks[1]._CurrentXcord) < MIN_X_DISTANCE &&
                    Math.Abs(x._InvolvedTracks[0]._CurrentYcord -
                             x._InvolvedTracks[1]._CurrentYcord) < MIN_Y_DISTANCE &&
                    Math.Abs(x._InvolvedTracks[0]._CurrentZcord -
                             x._InvolvedTracks[1]._CurrentZcord) < MIN_Z_DISTANCE);
                             */
        }

        public double CalculateTrackSpeed(TrackData newData, TrackData oldData)
        {
            //Check that the tags for the supplied data match
            if (!(newData._Tag.Equals(oldData._Tag)))
            {
                throw new System.ArgumentException("Tags for the supplied TrackData-objects do not match.");
            }

            //Get X, Y and Z for old data and new data
            double old_x = oldData._CurrentXcord;
            double new_x = newData._CurrentXcord;

            double old_y = oldData._CurrentYcord;
            double new_y = newData._CurrentYcord;

            //Omit Z-axis, since we're only interested in Horizontal velocity
            //double old_z = oldData._CurrentZcord;
            //double new_z = newData._CurrentZcord;

            //Get difference in X, Y and Z
            double dx = Math.Abs(old_x - new_x);
            double dy = Math.Abs(old_y - new_y);
            //double dz = Math.Abs(old_z - new_z);

            //Calculate distance traveled
            //double Distance = Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2) + Math.Pow(dz, 2));
            double Distance = Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));
            //Get the timestamps as strings
            string oldTime = oldData._TimeStamp;
            string newTime = newData._TimeStamp;

            //Format timestamps to DateTime
            string formatString = "yyyyMMddHHmmssfff";
            DateTime oldDateTime = DateTime.ParseExact(oldTime, formatString, null);
            DateTime newDateTime = DateTime.ParseExact(newTime, formatString, null);

            //Check that the new data is actually newer than the old data
            if (oldDateTime > newDateTime)
            {
                throw new System.ArgumentException("Timestamp of new data is older than Timestamp of old data");
            }

            //Get difference in DateTimes
            TimeSpan dt_TimeSpan = newDateTime - oldDateTime;
            int dt_ms = Math.Abs(dt_TimeSpan.Minutes * 1000*60) + Math.Abs(dt_TimeSpan.Seconds*1000) + Math.Abs(dt_TimeSpan.Milliseconds);


            //Calculate speed in m/s
            double speed = Distance / dt_ms * 1000;
            return speed;
        }

        public double CalculateTrackCourse(TrackData newData, TrackData oldData)
        {
            //Check that the tags for the supplied data match
            if (!(newData._Tag.Equals(oldData._Tag)))
            {
                throw new System.ArgumentException("Tags for the supplied TrackData-objects do not match.");
            }

            //Check that the new data is actually newer than the old data

            //Get the timestamps as strings
            string oldTime = oldData._TimeStamp;
            string newTime = newData._TimeStamp;

            //Format timestamps to DateTime
            string formatString = "yyyyMMddHHmmssfff";
            DateTime oldDateTime = DateTime.ParseExact(oldTime, formatString, null);
            DateTime newDateTime = DateTime.ParseExact(newTime, formatString, null);


            if (oldDateTime > newDateTime)
            {
                throw new System.ArgumentException("Timestamp of new data is older than Timestamp of old data");
            }

            //Get X and Y for old data and new data
            double old_x = oldData._CurrentXcord;
            double new_x = newData._CurrentXcord;

            double old_y = oldData._CurrentYcord;
            double new_y = newData._CurrentYcord;

            //Get difference in X and Y
            double dx = new_x-old_x;
            double dy = new_y - old_y;

            //Calculate angle in degrees
            double Angle = Math.Atan2(dy,dx)*(180/Math.PI);
            return Angle;
        }

        public void Update(TrackData trackdata)
        {
            HandleNewTrackData(trackdata);
        }

        public void UpdateTrackData(TrackData trackData)
        {
            // Update trackdata
            TrackData trackToEdit = _currentTracks.Find(x => x._Tag == trackData._Tag);
            trackToEdit._CurrentHorzVel = CalculateTrackSpeed(trackData, trackToEdit);
            trackToEdit._CurrentCourse = CalculateTrackCourse(trackData, trackToEdit);
            trackToEdit._CurrentXcord = trackData._CurrentXcord;
            trackToEdit._CurrentYcord = trackData._CurrentYcord;
            trackToEdit._CurrentZcord = trackData._CurrentZcord;
            trackToEdit._CurrentHorzVel = trackToEdit._CurrentHorzVel;
            trackToEdit._TimeStamp = trackData._TimeStamp;

            //Replace old object with new object
            int index = _currentTracks.FindIndex(x => x._Tag == trackData._Tag);
            _currentTracks.RemoveAt(index);
            _currentTracks.Insert(index, trackToEdit);
        }

        public void CheckIfTrackdataIsStillInAirspace(TrackData trackData)
        {
            if (!(_airspace.CheckIfInMonitoredArea(trackData._CurrentXcord, trackData._CurrentYcord,
                trackData._CurrentZcord)))
            {
                _currentEvents.AddTrackLeftEventFor(trackData,_outputFile);
                RemoveTrack(trackData._Tag);
            }
        }
    }
}
