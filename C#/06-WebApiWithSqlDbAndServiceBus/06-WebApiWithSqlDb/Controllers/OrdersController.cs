using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _06_WebApiWithSqlDb.Data;
using _06_WebApiWithSqlDb.Models;
using _06_WebApiWithSqlDb.ServiceHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace _06_WebApiWithSqlDb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private ServiceBusHelper helper;

        public OrdersController(ServiceBusHelper serviceBusHelper)
        {
            this.helper = serviceBusHelper;
        }

        [HttpPost("placeorder")]
        public async Task<ActionResult<Order>> PlaceOrder(Order order)
        {
            var messageText=JsonConvert.SerializeObject(order);
            await helper.SendMessageAsync(messageText);
            return Ok(order);
        }

        
    }
}