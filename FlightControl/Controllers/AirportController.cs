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
        public string GetLog()
        {
            return Information.GetLogPiece()?.Message;
        }
        [HttpGet]
        public string CloseStation(int num)
        {
            var result = Chain.CloseStation(num);
            return result.Message;
        }
        [HttpGet]
        public string OpenStation(int num)
        {
            var result = Chain.OpenStation(num);
            return result.Message;
        }
        
    }
}
