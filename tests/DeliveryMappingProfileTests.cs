using AutoMapper;
using Epinova.PostnordShipping;
using Xunit;

namespace Epinova.PostnordShippingTests
{
    public class DeliveryMappingProfileTests
    {
        [Fact]
        public void AllowNullCollections_IsFalse()
        {
            var profile = new DeliveryMappingProfile();
            Assert.False(profile.AllowNullCollections);
        }

        [Fact]
        public void AutoMapperConfiguration_IsValid()
        {
            var config = new MapperConfiguration(cfg => { cfg.AddProfile<DeliveryMappingProfile>(); });

            config.AssertConfigurationIsValid();
        }

        [Fact]
        public void Map_ServicePointDto_CorrectId()
        {
            var config = new MapperConfiguration(cfg => { cfg.AddProfile<DeliveryMappingProfile>(); });
            IMapper mapper = config.CreateMapper();

            var dto = new ServicePointDto { ServicePointId = Factory.GetString() };
            var result = mapper.Map<ServicePointInformation>(dto);

            Assert.Equal(dto.ServicePointId, result.Id);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Map_ServicePointDto_CorrectIsEligibleParcelOutlet(bool eligibleParcelOutlet)
        {
            var config = new MapperConfiguration(cfg => { cfg.AddProfile<DeliveryMappingProfile>(); });
            IMapper mapper = config.CreateMapper();

            var dto = new ServicePointDto { EligibleParcelOutlet = eligibleParcelOutlet };
            var result = mapper.Map<ServicePointInformation>(dto);

            Assert.Equal(dto.EligibleParcelOutlet, result.IsEligibleParcelOutlet);
        }

        [Fact]
        public void Map_ServicePointDto_CorrectNotificationPostalCodes()
        {
            var config = new MapperConfiguration(cfg => { cfg.AddProfile<DeliveryMappingProfile>(); });
            IMapper mapper = config.CreateMapper();

            var dto = new ServicePointDto { NotificationArea = new NotificationAreaDto { PostalCodes = new[] { Factory.GetString(), Factory.GetString() } } };
            var result = mapper.Map<ServicePointInformation>(dto);

            Assert.Collection(result.NotificationPostalCodes, x => Assert.Equal(dto.NotificationArea.PostalCodes[0], x), x => Assert.Equal(dto.NotificationArea.PostalCodes[1], x));
        }

        [Fact]
        public void Map_ServicePointDtoHasCoordinateArray_CorrectEasting()
        {
            var config = new MapperConfiguration(cfg => { cfg.AddProfile<DeliveryMappingProfile>(); });
            IMapper mapper = config.CreateMapper();

            var dto = new ServicePointDto { Coordinates = new[] { new CoordinateDto { Easting = Factory.GetInteger() / 3F } } };
            var result = mapper.Map<ServicePointInformation>(dto);

            Assert.Equal(dto.Coordinates[0].Easting, result.Easting);
        }

        [Fact]
        public void Map_ServicePointDtoHasCoordinateArray_CorrectId()
        {
            var config = new MapperConfiguration(cfg => { cfg.AddProfile<DeliveryMappingProfile>(); });
            IMapper mapper = config.CreateMapper();

            var dto = new ServicePointDto { Coordinates = new[] { new CoordinateDto { Id = Factory.GetString(7) } } };
            var result = mapper.Map<ServicePointInformation>(dto);

            Assert.Equal(dto.Coordinates[0].Id, result.CoordinateId);
        }

        [Fact]
        public void Map_ServicePointDtoHasCoordinateArray_CorrectNorthing()
        {
            var config = new MapperConfiguration(cfg => { cfg.AddProfile<DeliveryMappingProfile>(); });
            IMapper mapper = config.CreateMapper();

            var dto = new ServicePointDto { Coordinates = new[] { new CoordinateDto { Northing = Factory.GetInteger() / 3F } } };
            var result = mapper.Map<ServicePointInformation>(dto);

            Assert.Equal(dto.Coordinates[0].Northing, result.Northing);
        }

        [Fact]
        public void Map_ServicePointDtoHasSingleCoordinate_CorrectEasting()
        {
            var config = new MapperConfiguration(cfg => { cfg.AddProfile<DeliveryMappingProfile>(); });
            IMapper mapper = config.CreateMapper();

            var dto = new ServicePointDto { Coordinate = new CoordinateDto { Easting = Factory.GetInteger() / 3F } };
            var result = mapper.Map<ServicePointInformation>(dto);

            Assert.Equal(dto.Coordinate.Easting, result.Easting);
        }

        [Fact]
        public void Map_ServicePointDtoHasSingleCoordinate_CorrectId()
        {
            var config = new MapperConfiguration(cfg => { cfg.AddProfile<DeliveryMappingProfile>(); });
            IMapper mapper = config.CreateMapper();

            var dto = new ServicePointDto { Coordinate = new CoordinateDto { Id = Factory.GetString(5) } };
            var result = mapper.Map<ServicePointInformation>(dto);

            Assert.Equal(dto.Coordinate.Id, result.CoordinateId);
        }

        [Fact]
        public void Map_ServicePointDtoHasSingleCoordinate_CorrectNorthing()
        {
            var config = new MapperConfiguration(cfg => { cfg.AddProfile<DeliveryMappingProfile>(); });
            IMapper mapper = config.CreateMapper();

            var dto = new ServicePointDto { Coordinate = new CoordinateDto { Northing = Factory.GetInteger() / 3F } };
            var result = mapper.Map<ServicePointInformation>(dto);

            Assert.Equal(dto.Coordinate.Northing, result.Northing);
        }
    }
}
