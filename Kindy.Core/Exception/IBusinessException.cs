using System;
using System.Collections.Generic;
using System.Text;

namespace Kindy.Core.Exception
{
    public interface IBusinessException
    {
        string Message { get; }

        int ErrorCode { get; }

        object[] ErrorData { get; }
    }
}
