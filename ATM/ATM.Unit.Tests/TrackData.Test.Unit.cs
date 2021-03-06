﻿using ATM.Render;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATM.Unit.Tests
{
    [TestFixture]
    class TrackData_unit_test
    {
        IConsoleOutput fakeConsoleOutput;
        TrackData uut;

        [SetUp]
        public void setup()
        {
            fakeConsoleOutput = Substitute.For<IConsoleOutput>();
            uut = new TrackData("ABC123", 10000, 20000, 3000, "201118132700100", 100, 45, fakeConsoleOutput);
        }

        public void TrackData_FormatData_CorrectStringIsReturned()
        {
            string Tag = uut.Tag;
            double x = uut.CurrentXcord;
            double y = uut.CurrentYcord;
            double z = uut.CurrentZcord;
            double horzVel = uut.CurrentHorzVel;
            double course = uut.CurrentCourse;

            string expectedString = $"{Tag} - ( {x}, {y}, {z}) - Speed: {horzVel} m/s - Course: {course} degrees";
            Assert.That(() => uut.FormatData().Equals(expectedString));
        }

        public void TrackData_Render_RendererReceivedCorrectString()
        {
            string Tag = uut.Tag;
            double x = uut.CurrentXcord;
            double y = uut.CurrentYcord;
            double z = uut.CurrentZcord;
            double horzVel = uut.CurrentHorzVel;
            double course = uut.CurrentCourse;

            string expectedString = $"{Tag} - ( {x}, {y}, {z}) - Speed: {horzVel} m/s - Course: {course} degrees";

            uut.Render();

            fakeConsoleOutput.Received().Print(Arg.Is<string>(str => str.Contains(expectedString)));
        }
    }
}
