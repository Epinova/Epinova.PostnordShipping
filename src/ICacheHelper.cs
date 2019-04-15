using System;
using EPiServer.Framework.Cache;

namespace Epinova.PostnordShipping
{
    public interface ICacheHelper
    {
        T Get<T>(string key) where T : class;
        void Insert(string key, object value, TimeSpan timeToLive);
        void Insert(string key, object value, CacheEvictionPolicy evictionPolicy);
    }
}