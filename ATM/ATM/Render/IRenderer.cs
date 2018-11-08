using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATM.Events;

namespace ATM.Render
{
    public interface IRenderer
    {
        void RenderTrack(TrackData trackData);

        void RenderSeperationEvent(SeperationEvent seperationEvent);
    }
}
