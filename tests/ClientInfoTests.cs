using System;
using Epinova.PostnordShipping;
using Xunit;

namespace Epinova.PostnordShippingTests
{
    public class ClientInfoTests
    {
        [Fact]
        public void CacheTimeout_DefaultValue_IsTwoDays()
        {
            var clientInfo = new ClientInfo();

            Assert.Equal(TimeSpan.FromDays(2), clientInfo.CacheTimeout);
        }
    }
}