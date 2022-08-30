using Newtonsoft.Json.Serialization;
using System;

namespace Kindy.DDDTemplate.API.Extension.ControllerJson
{
    public class ExtendedCamelCaseContractResolver : CamelCasePropertyNamesContractResolver
    {
        private bool longAsString = false;
        /// <summary>
        /// 创建 <see cref="ExtendedCamelCaseContractResolver"/> 的新实例。
        /// </summary>
        /// <param name="useLongAsString">是否将 long 序列化为 string 类型（javascript 无法使用 64 位整数）。</param>
        public ExtendedCamelCaseContractResolver(bool useLongAsString = false) : base()
        {
            longAsString = useLongAsString;
        }

        private static bool IsLongOrNullableLong(Type objectType)
        {
            return objectType.Equals(typeof(long)) || objectType.Equals(typeof(long?));
        }

        protected override JsonPrimitiveContract CreatePrimitiveContract(Type objectType)
        {
            if (IsLongOrNullableLong(objectType) && longAsString)
            {
                return new LongToStringContract(objectType);
            }
            return base.CreatePrimitiveContract(objectType);
        }
    }
}
