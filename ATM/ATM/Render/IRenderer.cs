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
        string RenderTrack(TrackData trackData);

        void RenderEvent(Event Event);
    }
}
