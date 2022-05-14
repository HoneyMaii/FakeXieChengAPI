using System.Collections.Generic;

namespace FakeXieCheng.API.Services
{
    public class PropertyMappingValue
    {
        public IEnumerable<string> DestinationProperties { get; set; } // 将会被映射的目标类型属性

        // 构造函数
        public PropertyMappingValue(IEnumerable<string> destinationProperties)
        {
            DestinationProperties = destinationProperties;
        }
    }
}