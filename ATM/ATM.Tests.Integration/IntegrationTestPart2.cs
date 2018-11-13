using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATM.Events;
using ATM.Logger;
using ATM.Render;
using NUnit.Framework;

namespace ATM.Tests.Integration
{
    [TestFixture]
    class IntegrationTestPart2
    {
        //S's - Stubs
        private IConsoleOutput fakeConsoleOutput;
        private IFileOutput fakeFileOutput;
        private FileLogger fakeFileLogger;
        private ConsoleRenderer fakeConsoleRenderer;
        //private Timer timer;

        //X's - Modules under test
        private TrackData trackData1;
        private TrackData trackData2;
        private List<TrackData> tracks;
        private SeperationEvent seperationEvent;
        private Airspace airspace;
        private TrackEnteredEvent trackEnteredEvent;
        private TrackLeftEvent trackLeftEvent;
        
        //T's - Modules acted upon.
        private ATMclass atmClass;

    }
}
