using Api.Persistence;
using Library.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TicketController : ControllerBase
    {
        private object _lock;
        private readonly ILogger<TicketController> _logger;


        [HttpGet]
        public IEnumerable<SupportTicket> Get()
        {
            return DataRepo.Current.Tickets;
        }

        [HttpGet("GetItem")]
        public Item GetTestItem()
        {
            return new SupportTicket();
        }

        [HttpPost("AddOrUpdate")]
        public Item Receive([FromBody] SupportTicket todo)
        {
            _lock = new object();
            lock (_lock)
            {
                DataRepo.Current.AddOrUpdate(todo);
            }
            return todo;
        }
        [HttpPost("Delete")]
        public void Delete([FromBody] SupportTicket ticket)
        {
            DataRepo.Current.Delete(ticket);
        }
    }
}