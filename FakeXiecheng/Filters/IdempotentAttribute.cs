using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;

namespace FakeXieCheng.API.Filters
{
    public class IdempotentAttribute : Attribute, IFilterFactory
    {
        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            var distributedCache = (IDistributedCache) serviceProvider.GetService(typeof(IDistributedCache));

            var filter = new IdempotentAttributeFilter(distributedCache);
            return filter;
        }

        public bool IsReusable => false;
    }
}