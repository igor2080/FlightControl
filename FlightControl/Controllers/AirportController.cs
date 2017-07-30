using FlightControl.Logic;
using FlightControl.Simulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FlightControl.Controllers
{
    public class AirportController : ApiController
    {
        [HttpGet]
        public bool AddPlane(int state)
        {
            var info = Chain.AcceptPlane(state==0);
            return info.Code == InfoCode.Success;
            
        }
        [HttpGet]
        public Information GetLog()
        {
            return Information.GetLogPiece();
        }
        [HttpGet]
        public string CloseStation(int station)
        {
            var result = Chain.CloseStation(station);
            return result.Message;
        }
        [HttpGet]
        public string OpenStation(int station)
        {
            var result = Chain.OpenStation(station);
            return result.Message;
        }
        [HttpGet]
        public Chain.SlotInfo GetStation(int station)
        {
            var result = Chain.GetStationInfo(station);
            return result;
        }
        [HttpGet]
        public void EmergencyLand(int station)
        {
            Chain.EmergencyLanding(station);
        }
    }
}
