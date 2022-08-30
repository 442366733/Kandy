using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kindy.DDDTemplate.Application.Queries.Order
{
    public class MyOrderQuery : IRequest<List<string>>
    {
        public string UserName { get; set; }
    }
}
