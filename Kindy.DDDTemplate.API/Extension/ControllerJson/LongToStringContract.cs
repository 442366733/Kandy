using Newtonsoft.Json.Serialization;
using System;

namespace Kindy.DDDTemplate.API.Extension.ControllerJson
{
    public class LongToStringContract : JsonPrimitiveContract
    {
        private static WeakReference<LongToStringJsonConverter> _longToString;

        private static LongToStringJsonConverter GetLongToStringConverter()
        {
            if (_longToString == null)
            {
                LongToStringJsonConverter contract = new LongToStringJsonConverter();
                _longToString = new WeakReference<LongToStringJsonConverter>(contract);
            }
            if (_longToString.TryGetTarget(out LongToStringJsonConverter c))
            {
                return c;
            }
            _longToString = null;
            return GetLongToStringConverter();
        }

        public LongToStringContract(Type underlyingType) : base(underlyingType)
        {
            Converter = GetLongToStringConverter();
        }
    }
}
