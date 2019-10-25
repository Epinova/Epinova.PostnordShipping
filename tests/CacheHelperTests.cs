using System;
using Epinova.PostnordShipping;
using EPiServer.Framework.Cache;
using EPiServer.Logging;
using Moq;
using Xunit;

namespace Epinova.PostnordShippingTests
{
    public class CacheHelperTests
    {
        private readonly CacheHelper _cacheHelper;
        private readonly Mock<ISynchronizedObjectInstanceCache> _cacheManagerMock;

        public CacheHelperTests()
        {
            var logMock = new Mock<ILogger>();
            _cacheManagerMock = new Mock<ISynchronizedObjectInstanceCache>();
            _cacheHelper = new CacheHelper(logMock.Object, _cacheManagerMock.Object);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Get_KeyIsInvalid_DoesNotCallCacheManager(string cacheKey)
        {
            _cacheHelper.Get<object>(cacheKey);

            _cacheManagerMock.Verify(m => m.Get(cacheKey), Times.Never());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Get_KeyIsInvalid_ReturnsDafaultValue(string cacheKey)
        {
            var fromCache = _cacheHelper.Get<object>(cacheKey);

            Assert.Equal(default(object), fromCache);
        }

        [Fact]
        public void Get_KeyIsValid_CallsCacheManager()
        {
            string cacheKey = Factory.GetString();
            var fromCache = _cacheHelper.Get<object>(cacheKey);

            _cacheManagerMock.Verify(m => m.Get(cacheKey), Times.Once());
        }

        [Fact]
        public void Get_KeyIsValid_GetsObjectFromCache()
        {
            string cacheKey = Factory.GetString();
            var objectInCache = new object();
            _cacheManagerMock.Setup(m => m.Get(cacheKey)).Returns(objectInCache);

            var fromCache = _cacheHelper.Get<object>(cacheKey);
            Assert.Same(objectInCache, fromCache);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Insert_KeyIsInvalid_DoesNotCallCacheManager(string cacheKey)
        {
            var objectToCache = new object();
            TimeSpan timeToLive = TimeSpan.FromSeconds(Factory.GetInteger());

            _cacheHelper.Insert(cacheKey, objectToCache, timeToLive);

            _cacheManagerMock.Verify(m => m.Insert(cacheKey, objectToCache, It.IsAny<CacheEvictionPolicy>()), Times.Never());
        }

        [Fact]
        public void Insert_KeyIsValid_CallCacheManagerWithCorrectTimeoutPolicy()
        {
            string cacheKey = Factory.GetString();
            var objectToCache = new object();
            TimeSpan timeToLive = TimeSpan.FromSeconds(Factory.GetInteger());

            _cacheHelper.Insert(cacheKey, objectToCache, timeToLive);

            _cacheManagerMock.Verify(m => m.Insert(cacheKey, objectToCache, It.Is<CacheEvictionPolicy>(p => p.TimeoutType == CacheTimeoutType.Absolute)), Times.Once());
        }

        [Fact]
        public void Insert_KeyIsValid_CallCacheManagerWithCorrectTimeToLivePolicy()
        {
            string cacheKey = Factory.GetString();
            var objectToCache = new object();
            TimeSpan timeToLive = TimeSpan.FromSeconds(Factory.GetInteger());

            _cacheHelper.Insert(cacheKey, objectToCache, timeToLive);

            _cacheManagerMock.Verify(m => m.Insert(cacheKey, objectToCache, It.Is<CacheEvictionPolicy>(p => p.Expiration == timeToLive)), Times.Once());
        }
    }
}
