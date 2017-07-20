using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FlightControl.Data;
using System.Collections.Generic;
using FlightControl.Simulator;
using FlightControl.Logic;
using System.Threading;

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
            var plane = Simulation.GeneratePlane( true);
            Assert.IsTrue(plane.ID>0 && plane.LifeSpan>DateTime.MinValue && plane.IsWorking);
        }

        [TestMethod]
        public void SystemInitialization()
        {
            var result = Chain.GetInfo(1);
            Assert.IsNotNull(result.CurrentAirplane);
        }
        
        [TestMethod]
        public void DepartingPlaneMovement()
        {
            var plane = Simulation.GeneratePlane(false);
            Chain.AcceptPlane(plane);
            Chain.MovePlane(plane.ID);
            Assert.IsTrue(plane.ID == Chain.GetInfo(6).CurrentAirplane.ID);
        }

    }
}
