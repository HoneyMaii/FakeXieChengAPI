using Microsoft.Extensions.Configuration;

namespace FakeXieCheng.API.ConfigurationCustom
{
    public class MyConfigurationSource: IConfigurationSource
    {
        // 返回具体的 Provider
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new MyConfigurationProvider();
        }
    }
}