using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATM.Logger;
using ATM.Render;

namespace ATM.Events
{
    public abstract class Event
    {
        public string _occurrenceTime { get; set; }
        public List<TrackData> _InvolvedTracks { get; set; }
        public bool _isRaised { get; set; }
        private ILogger _logger;
        private IRenderer _renderer;

        private Event(IRenderer renderer, ILogger logger)
        {
            _logger = logger;
            _renderer = renderer;
        }

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

        public void Log()
        {
            // need to use render
            Console.WriteLine(FormatData());
        }

        public void Render()
        {
            _renderer.RenderEvent(this);
            
        }
    }
}
