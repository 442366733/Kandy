using Kindy.Infrastructure.Core.Context;
using Microsoft.Extensions.Logging;

namespace Kindy.DDDTemplate.Infrastructure.Context.Master
{
    /// <summary>
    /// 对应事物处理
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class MasterDBTransactionBehavior<TRequest, TResponse> : TransactionBehavior<MasterDBContext, TRequest, TResponse>
    {
        public MasterDBTransactionBehavior(MasterDBContext dbContext, ILogger<MasterDBTransactionBehavior<TRequest, TResponse>> logger) : base(dbContext, logger)
        {
        }
    }
}
