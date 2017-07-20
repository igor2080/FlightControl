using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FlightControl.Data;
using System.Collections.Generic;
using FlightControl.Simulator;
using FlightControl.Logic;

namespace FlightControl.Test
{
    [TestClass]
    public class ProjectTests
    {
        private TestContext testContextInstance;

        public ProjectTests()
        {
            Main.Start();
        }
        /// <summary>
        ///  Gets or sets the test context which provides
        ///  information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        [TestMethod]
        public void PlaneGeneration()
        {
            var plane = Simulation.GeneratePlane(true);
            Assert.IsTrue(plane.ID > 0 &&
                plane.LifeSpan > DateTime.MinValue &&
                plane.IsWorking);
        }

        [TestMethod]
        public void SystemInitialization()
        {
            var result = Chain.GetPlaneInfo(1);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void AddingDepartingPlane()
        {
            var plane = Chain.AcceptPlane(false);
            Assert.IsTrue(plane.Code==InfoCode.Success);
        }


    }
}
