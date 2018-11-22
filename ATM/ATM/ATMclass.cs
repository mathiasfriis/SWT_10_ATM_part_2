using System;
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
        readonly double MIN_X_DISTANCE = 5000;
        readonly double MIN_Y_DISTANCE = 5000;
        readonly double MIN_Z_DISTANCE = 300;


        public IConsoleOutput _outputConsole;
        public IFileOutput _outputFile;

        private IAirspace _airspace;
        public ITransponderReceiver _transponderReceiver;

        public List<TrackData> CurrentTracks { get; }
        public EventList CurrentEvents { get; }

        private static Timer Timer;

        public ATMclass(IConsoleOutput outputConsole, IFileOutput outputFile, IAirspace airspace, ITransponderReceiver transponderReceiver)

        {
            _outputConsole = outputConsole;
            _outputFile = outputFile;
            _transponderReceiver = transponderReceiver;
            _airspace = airspace;
            CurrentEvents = new EventList();
            CurrentTracks = new List<TrackData>();

            Timer = new System.Timers.Timer
            {
                Interval = 100
            };
            Timer.Elapsed += TimerElapsed;
            Timer.AutoReset = true;
            Timer.Enabled = true;

        }

        public void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            _outputConsole.Clear();
            CurrentEvents.cleanUpEvents();
            RenderEvents();
            RenderTracks();
        }


        public void HandleNewTrackData(TrackData trackdata)
        {
            //Check if no track data with given tag exists.
            if (CurrentTracks.Exists(x => x.Tag == trackdata.Tag) == false)
            {
                // Add the new track if coordinates are inside the given boundaries of the airspace.
                if (_airspace.CheckIfInMonitoredArea(trackdata.CurrentXcord, trackdata.CurrentYcord,
                    trackdata.CurrentZcord))
                {
                    AddTrack(trackdata);
                    //string time = trackdata._TimeStamp;
                    //TrackEnteredEvent TrackEnteredEvent = new TrackEnteredEvent(time, trackdata, true, _outputConsole, _outputFile);
                    CurrentEvents.AddTrackEnteredEventFor(trackdata, _outputFile);
                    
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
            CurrentEvents.cleanUpEvents();

            // Check for potential seperation events
            CheckForSeperationEvents(trackdata);

            // Remove separations event after update
            UpdateSeperationEventStatus();

        }

        public void AddTrack(TrackData trackData)
        {
            //Check if TrackData with given tag already exists.
            if(CurrentTracks.Exists(x => x.Tag == trackData.Tag))
            {
                //Find index of existing data.
                int index = CurrentTracks.FindIndex(x => x.Tag == trackData.Tag);
                //replace existing data with new data.
                CurrentTracks[index] = trackData;
            }
            else
            {
                //Add trackData.
                CurrentTracks.Add(trackData);
            }
            
        }

        public void RemoveTrack(string tag)
        {
            int index = CurrentTracks.FindIndex(x => x.Tag.Equals(tag));
            CurrentTracks.RemoveAt(index);
        }

        public void CheckForSeperationEvents(TrackData trackData)
        {
            foreach (var track in CurrentTracks)
            {
                if (trackData != null && track.Tag != trackData.Tag)
                {
                    if(CheckForSeperationEventConditions(trackData, track)==true)
                    {
                        //If seperation event does not exist yet, create it and add it to currentSeperationEvents
                        //Check if separation event already exists
                        if (!CurrentEvents.CheckIfSeperationEventExistsFor(trackData, track))
                        {
                            // Add new separation event 
                            //string time = DateTime.Now.ToString();
                            string time = trackData.TimeStamp;
                            List<TrackData> trackDataInSeperationEvent = new List<TrackData>
                            {
                                trackData,
                                track
                            };

                            //_currentEvents.Add(SeperationEvent);
                            CurrentEvents.AddSeperationEventFor(trackData, track, _outputFile);

                            //Create event to log
                            SeperationEvent SeperationEvent = new SeperationEvent(time, trackDataInSeperationEvent, true, _outputConsole, _outputFile);
                            SeperationEvent.Log();

                            CurrentEvents.AddSeperationEventFor(trackData, track, _outputFile);
                        }
                    }
                }   
            }
        }

        public bool CheckForSeperationEventConditions(TrackData trackData1, TrackData trackData2)
        {
            //Check if both tracks are the same
            if(trackData1.Tag==trackData2.Tag)
            {
                throw new Exception("Provided TrackDatas have the same Tag");
            }
            else
            {
                //Check if conditions are met with the given TrackData-objects
                if (Math.Abs(trackData1.CurrentXcord - trackData2.CurrentXcord) < MIN_X_DISTANCE &&
                    Math.Abs(trackData1.CurrentYcord - trackData2.CurrentYcord) < MIN_Y_DISTANCE &&
                    Math.Abs(trackData1.CurrentZcord - trackData2.CurrentZcord) < MIN_Z_DISTANCE)
                {
                    return true;
                }

                else
                    return false;
            }
        }

        public void RenderEvents()
        {

            //Console.WriteLine("Number of separation events: " + _currentEvents.events.OfType<SeperationEvent>().Count());

           CurrentEvents.RenderEvents();
            _outputConsole.Print("Number of separation events: " + CurrentEvents.events.OfType<SeperationEvent>().Count());
        }

        public void RenderTracks()
        {
            //Console.Clear();

            foreach (var trackData in CurrentTracks)
            {
                trackData.Render();
            }

        }

        public void UpdateSeperationEventStatus()
        {
            //Log if conditions for seperation event are no longer met.
            foreach (var e in CurrentEvents.events)
            {
                if (e is SeperationEvent)
                {
                    string tag0 = e.InvolvedTracks[0].Tag;
                    string tag1 = e.InvolvedTracks[1].Tag;

                    TrackData track0;
                    TrackData track1;

                    //get updated track data
                    try
                    {
                        track0 = CurrentTracks.Find(x => x.Tag == tag0);
                        track1 = CurrentTracks.Find(x => x.Tag == tag1);
                        //if the updated track data no longer matches conditions for SeperationEvent, set the isRaise-attribute to false
                        if (CheckForSeperationEventConditions(track0, track1) == false)
                        {
                            //Mark that seperation event is no longer active - It will now be removed at next "cleanUp"
                            e.isRaised = false;
                        }
                    }
                    catch
                    {
                        //Mark that seperation event is no longer active - It will now be removed at next "cleanUp"
                        e.isRaised = false;
                    }
                    

                    
                }
                
            }

        }

        public double CalculateTrackSpeed(TrackData newData, TrackData oldData)
        {
            //Check that the tags for the supplied data match
            if (!(newData.Tag.Equals(oldData.Tag)))
            {
                throw new System.ArgumentException("Tags for the supplied TrackData-objects do not match.");
            }

            //Get X, Y and Z for old data and new data
            double old_x = oldData.CurrentXcord;
            double new_x = newData.CurrentXcord;

            double old_y = oldData.CurrentYcord;
            double new_y = newData.CurrentYcord;

            //Get difference in X, Y and Z
            double dx = Math.Abs(old_x - new_x);
            double dy = Math.Abs(old_y - new_y);

            //Calculate distance traveled
            double Distance = Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));
            //Get the timestamps as strings
            string oldTime = oldData.TimeStamp;
            string newTime = newData.TimeStamp;

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
            if (!(newData.Tag.Equals(oldData.Tag)))
            {
                throw new System.ArgumentException("Tags for the supplied TrackData-objects do not match.");
            }

            //Check that the new data is actually newer than the old data
            //Get the timestamps as strings
            string oldTime = oldData.TimeStamp;
            string newTime = newData.TimeStamp;

            //Format timestamps to DateTime
            string formatString = "yyyyMMddHHmmssfff";
            DateTime oldDateTime = DateTime.ParseExact(oldTime, formatString, null);
            DateTime newDateTime = DateTime.ParseExact(newTime, formatString, null);


            if (oldDateTime > newDateTime)
            {
                throw new System.ArgumentException("Timestamp of new data is older than Timestamp of old data");
            }

            //Get X and Y for old data and new data
            double old_x = oldData.CurrentXcord;
            double new_x = newData.CurrentXcord;

            double old_y = oldData.CurrentYcord;
            double new_y = newData.CurrentYcord;

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
            TrackData trackToEdit = CurrentTracks.Find(x => x.Tag == trackData.Tag);
            trackToEdit.CurrentHorzVel = CalculateTrackSpeed(trackData, trackToEdit);
            trackToEdit.CurrentCourse = CalculateTrackCourse(trackData, trackToEdit);
            trackToEdit.CurrentXcord = trackData.CurrentXcord;
            trackToEdit.CurrentYcord = trackData.CurrentYcord;
            trackToEdit.CurrentZcord = trackData.CurrentZcord;
            trackToEdit.CurrentHorzVel = trackToEdit.CurrentHorzVel;
            trackToEdit.TimeStamp = trackData.TimeStamp;

            //Replace old object with new object
            int index = CurrentTracks.FindIndex(x => x.Tag == trackData.Tag);
            CurrentTracks.RemoveAt(index);
            CurrentTracks.Insert(index, trackToEdit);
        }

        public void CheckIfTrackdataIsStillInAirspace(TrackData trackData)
        {
            if (!(_airspace.CheckIfInMonitoredArea(trackData.CurrentXcord, trackData.CurrentYcord,
                trackData.CurrentZcord)))
            {
                CurrentEvents.AddTrackLeftEventFor(trackData,_outputFile);
                RemoveTrack(trackData.Tag);
            }
        }
    }
}
