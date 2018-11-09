using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATM.Events;
using ATM;
using ATM.Logger;
using ATM.Render;
using NSubstitute;
using NUnit.Framework;

namespace ATM.Tests.Integration
{
    [TestFixture]
    public class IntegrationTestPart1
    {
        private FileLogger logger;
        private ConsoleRenderer renderer;
        private IConsoleOutput fakeConsoleOutput;
        private IFileOutput fakeFileOutput;
        private TrackData trackData;
        private SeperationEvent seperationEvent;
        private TrackEnteredEvent trackEnteredEvent;
        private TrackLeftEvent trackLeftEvent;



        [SetUp]
        public void setup()
        {
            //Set up stubs
            fakeConsoleOutput = Substitute.For<IConsoleOutput>();
            fakeFileOutput = Substitute.For<IFileOutput>();

            //Set up modules under test

            //Set up already tested modules
        }
    }
}
