using System;
using EPiServer.Framework.Cache;
using EPiServer.Logging;

namespace Epinova.PostnordShipping
{
    public class CacheHelper : ICacheHelper
    {
        private readonly ISynchronizedObjectInstanceCache _cacheManager;
        private readonly ILogger _log;

        public CacheHelper(ILogger log, ISynchronizedObjectInstanceCache cacheManager)
        {
            _log = log;
            _cacheManager = cacheManager;
        }

        public T Get<T>(string key) where T : class
        {
            if (String.IsNullOrWhiteSpace(key))
                return default(T);

            return _cacheManager.Get(key) as T;
        }

        public void Insert(string key, object value, TimeSpan timeToLive)
        {
            if (String.IsNullOrWhiteSpace(key))
                return;
            var evictionPolicy = new CacheEvictionPolicy(timeToLive, CacheTimeoutType.Absolute);

            _log.Debug(evictionPolicy, ep => $"Key: {key}, Item: {value}, CacheKeys: {ep?.CacheKeys}, Type: {ep?.TimeoutType}, Seconds: {ep?.Expiration.Duration().TotalSeconds}");

            _cacheManager.Insert(key, value, evictionPolicy);
        }
    }
}