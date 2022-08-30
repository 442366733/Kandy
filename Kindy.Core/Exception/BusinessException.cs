namespace Kindy.Core.Exception
{
    public class BusinessException : IBusinessException
    {
        public string Message { get; private set; }

        public int ErrorCode { get; private set; }

        public object[] ErrorData { get; private set; }

        public readonly static IBusinessException Unknown = new BusinessException { Message = "未知错误", ErrorCode = 9999 };

        public static BusinessException FromBusinessException(IBusinessException exception)
        {
            return new BusinessException { Message = exception.Message, ErrorCode = exception.ErrorCode, ErrorData = exception.ErrorData };
        }
    }
}
