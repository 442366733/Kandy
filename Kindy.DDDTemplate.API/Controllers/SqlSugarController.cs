using DotNetCore.CAP;
using DotNetCore.CAP.Messages;
using Kindy.DatabaseAccessor.SqlSugar.Repositories;
using Kindy.DDDTemplate.Domain.Aggregates.OrderAggregates;
using Kindy.EventBusClient.Rabbitmq;
using Kindy.EventBusClient.Rabbitmq.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Kindy.DDDTemplate.API.Controllers
{
    /// <summary>
    /// SqlSugar控制器
    /// </summary>
    //[Authorize]
    [ApiController]
    [Route("[controller]")]
    public class SqlSugarController : ControllerBase
    {
        private readonly ILogger<SqlSugarController> _logger;
        private readonly ISqlSugarRepository<Order> _orderRepository;
        private readonly IServiceProvider _serviceProvider;
        public SqlSugarController(
            ILogger<SqlSugarController> logger,
            ISqlSugarRepository<Order> orderRepository,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _orderRepository = orderRepository;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 查询订单信息
        /// </summary>
        /// <param name="id">主键id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> Get(int id)
        {
            var result = await _orderRepository.FirstOrDefaultAsync(x => x.id == id);
            _logger.LogInformation(JsonConvert.SerializeObject(result));
            return result;
        }
    }
}
