using Epinova.PostnordShipping;
using StructureMap;
using Xunit;
using Xunit.Abstractions;

namespace Epinova.PostnordShippingTests
{
    public class DeliveryRegistryTests
    {
        private readonly Container _container;
        private readonly ITestOutputHelper _output;

        public DeliveryRegistryTests(ITestOutputHelper output)
        {
            _output = output;
            _container = new Container(new TestableRegistry());
            _container.Configure(x => { x.AddRegistry(new DeliveryRegistry()); });
        }

        [Fact]
        public void AssertConfigurationIsValid()
        {
            _output.WriteLine(_container.WhatDoIHave());
            _container.AssertConfigurationIsValid();
        }

        [Fact]
        public void Getting_ICacheHelper_ReturnsCacheHelper()
        {
            var instance = _container.GetInstance<ICacheHelper>();

            Assert.IsType<CacheHelper>(instance);
        }

        [Fact]
        public void Getting_IDeliveryService_ReturnsDeliveryService()
        {
            var instance = _container.GetInstance<IDeliveryService>();

            Assert.IsType<DeliveryService>(instance);
        }
    }
}