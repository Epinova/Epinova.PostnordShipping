using EPiServer.Framework.Cache;
using EPiServer.Logging;
using Moq;
using StructureMap;

namespace Epinova.PostnordShippingTests
{
    internal class TestableRegistry : Registry
    {
        public TestableRegistry()
        {
            For<ILogger>().Use(new Mock<ILogger>().Object);
            For<ISynchronizedObjectInstanceCache>().Use(new Mock<ISynchronizedObjectInstanceCache>().Object);
        }
    }
}
