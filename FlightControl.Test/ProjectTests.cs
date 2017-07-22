using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FlightControl.Data;
using System.Collections.Generic;
using FlightControl.Simulator;
using FlightControl.Logic;
using System.Net;
using System.Text;
using System.Threading;
using System.Collections;

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
            Assert.IsTrue(plane.ID > 100000 &&
                plane.LifeSpan > DateTime.MinValue &&
                plane.IsWorking);
        }

        [TestMethod]
        public void SystemInitialization()
        {
            var result = Information.GetLogs(InfoCode.Started)[0];
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void AddingDepartingPlane()
        {
            var plane = Chain.AcceptPlane(false);
            Assert.IsTrue(plane.Code == InfoCode.Success);
        }

        [TestMethod]
        public void Closing_A_Station()
        {
            var result = Chain.CloseStation(6);
            Assert.IsTrue(result.Code == InfoCode.Closed);
        }
        [TestMethod]
        public void Closing_A_Closed_Station()
        {
            Chain.CloseStation(6);
            var result = Chain.CloseStation(6);
            Assert.IsTrue(result.Code == InfoCode.Error);
        }

        [TestMethod]
        public void Opening_A_Station()
        {
            Chain.CloseStation(6);
            var result = Chain.OpenStation(6);
            Assert.IsTrue(result.Code == InfoCode.Open);
        }
        [TestMethod]
        public void Opening_An_Open_Station()
        {
            var result = Chain.OpenStation(6);
            Assert.IsTrue(result.Code == InfoCode.Error);
            
        }
       


    }
}
