using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace FakeXieCheng.API.Filters
{
    public class IdempotentAttributeFilter : IActionFilter, IResultFilter
    {
        private readonly IDistributedCache _distributedCache;
        private bool _isIdempotencyCache; // 幂等键是否在缓存中
        private const string IdempotencyKeyHeaderName = "IdempotencyKey";
        private string _idempotencyKey;

        //  依赖注入
        public IdempotentAttributeFilter(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            Microsoft.Extensions.Primitives.StringValues idempotencyKeys;
            // 请求头中获取幂等键：IdempotencyKey
            context.HttpContext.Request.Headers.TryGetValue(IdempotencyKeyHeaderName, out idempotencyKeys);
            _idempotencyKey = idempotencyKeys.ToString();

            // 缓存中获取本次请求的幂等键并做判断
            var cacheData = _distributedCache.GetString(GetDistributedCacheKey());
            if (cacheData != null)
            {
                context.Result = JsonConvert.DeserializeObject<ObjectResult>(cacheData);
                _isIdempotencyCache = true;
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            //  幂等键已缓存
            if (_isIdempotencyCache) return;

            var contextResult = context.Result;
            var cacheEntryOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = new TimeSpan(24, 0, 0)
            };
            //  设置缓存
            _distributedCache.SetString(GetDistributedCacheKey(), JsonConvert.SerializeObject(contextResult),
                cacheEntryOptions);
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
        }

        private string GetDistributedCacheKey()
        {
            return $"Idempotency:{_idempotencyKey}";
        }
    }
}