using FakeXieCheng.API.ConfigurationCustom;

namespace Microsoft.Extensions.Configuration // 方便引用时直接使用
{
    // 只需要暴露扩展方法，不需要暴露 MyConfigurationSource
    public static class MyConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddMyConfiguration(this IConfigurationBuilder builder)
        {
            builder.Add(new MyConfigurationSource());
            return builder;
        }
    }
}