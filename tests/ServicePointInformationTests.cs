using Epinova.PostnordShipping;
using Xunit;

namespace Epinova.PostnordShippingTests
{
    public class ServicePointInformationTests
    {
        [Fact]
        public void Ctor_OpeningHours_IsNotNull()
        {
            var model = new ServicePointInformation();
            Assert.NotNull(model.OpeningHours);
        }
    }
}
