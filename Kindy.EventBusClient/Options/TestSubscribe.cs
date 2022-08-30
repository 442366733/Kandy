using System;
using System.Collections.Generic;
using System.Text;

namespace Kindy.EventBusClient.Rabbitmq.Options
{
    public class TestSubscribe : IEventBusService
    {
        //[Subscribe("test.rk")]
        //public void Handle(TestEvent a)
        //{
        //    Console.WriteLine("订阅信息:" + a);
        //}

        //[Subscribe("test.rk2")]
        //public void Handle2(TestEvent a)
        //{
        //    Console.WriteLine("订阅信息:" + a);
        //}
    }

    public class TestEvent : IntegrationEvent
    {
        public string Message { get; set; }
    }
}
