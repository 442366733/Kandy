using Kindy.Infrastructure.Core.Context;
using Microsoft.Extensions.Logging;

namespace Kindy.DDDTemplate.Infrastructure.Context.CRM
{
    /// <summary>
    /// 对应事物处理
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class CRMDBTransactionBehavior<TRequest, TResponse> : TransactionBehavior<CRMDBContext, TRequest, TResponse>
    {
        public CRMDBTransactionBehavior(CRMDBContext dbContext, ILogger<CRMDBTransactionBehavior<TRequest, TResponse>> logger) : base(dbContext, logger)
        {
        }
    }
}
