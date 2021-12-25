using Library.Models;
using Api.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AppointmentController : ControllerBase
    {
        private object _lock;
        [HttpGet("GetItem")]
        public Item GetTestItem()
        {
            return new Appointment();
        }

        [HttpGet]
        public IEnumerable<Appointment> Get()
        {
            return DataRepo.Current.Appointments;
        }

        [HttpPost("AddOrUpdate")]
        public Appointment AddOrUpdate([FromBody] Appointment appointment)
        {
            _lock = new object();
            lock (_lock)
            {
                DataRepo.Current.AddOrUpdate(appointment);
            }
            return appointment;
        }

        [HttpPost("Delete")]
        public void Delete([FromBody] Appointment appointment)
        {
            DataRepo.Current.Delete(appointment);
        }
    }
}