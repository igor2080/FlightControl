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
            var planes = new List<Airplane>();
            int curPlaneCount = planes.Count;
            Simulation.GeneratePlane(planes, true);
            Assert.IsTrue(curPlaneCount < planes.Count);
        }

        [TestMethod]
        public void InitializedChain()
        {
            var result = Chain.GetInfo(5);
            Assert.AreEqual((6, null), result);
        }
        [TestMethod]
        public void PlaneMovement()
        {
            List<Airplane> planes = new List<Airplane>();
            Simulation.GeneratePlane(planes, true);
            TestContext.WriteLine(Chain.AcceptPlane(planes[0]).ToString());
            TestContext.WriteLine(Chain.MovePlane(planes[0].ID).ToString());
            Assert.IsTrue(planes[0].ID == Chain.GetInfo(2).CurrentAirplane.ID);
        }

    }
}
