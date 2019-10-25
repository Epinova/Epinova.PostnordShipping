using Epinova.PostnordShipping;
using Xunit;

namespace Epinova.PostnordShippingTests
{
    public class ServicePointDtoTests
    {
        [Fact]
        public void Ctor_Coordinates_IsNotNull()
        {
            var dto = new ServicePointDto();
            Assert.NotNull(dto.Coordinates);
        }

        [Fact]
        public void Ctor_OpeningHours_IsNotNull()
        {
            var dto = new ServicePointDto();
            Assert.NotNull(dto.OpeningHours);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void GetCoordinate_CoordinateAndCoordinateArrayIsNull_ReturnEmptyCoordinate(bool arrayIsNull)
        {
            var dto = new ServicePointDto
            {
                Coordinate = null,
                Coordinates = arrayIsNull ? null : new CoordinateDto[0]
            };

            CoordinateDto coordinate = dto.GetCoordinate();

            Assert.Null(coordinate.Id);
            Assert.Equal(0F, coordinate.Northing);
            Assert.Equal(0F, coordinate.Easting);
        }

        [Fact]
        public void GetCoordinate_CoordinateIsNotNull_UseIgnoreCoodinatesArray()
        {
            var dto = new ServicePointDto
            {
                Coordinate = new CoordinateDto { Easting = Factory.GetInteger() / 3F, Northing = Factory.GetInteger() / 3F, Id = Factory.GetString(5) },
                Coordinates = new[] { new CoordinateDto { Easting = Factory.GetInteger() / 3F, Northing = Factory.GetInteger() / 3F, Id = Factory.GetString(5) } }
            };

            Assert.Equal(dto.Coordinate, dto.GetCoordinate());
        }

        [Fact]
        public void GetCoordinate_CoordinateIsNull_UseFirstFromCoodinatesArray()
        {
            var dto = new ServicePointDto { Coordinates = new[] { new CoordinateDto { Easting = Factory.GetInteger() / 3F, Northing = Factory.GetInteger() / 3F, Id = Factory.GetString(5) } } };

            Assert.Equal(dto.Coordinates[0], dto.GetCoordinate());
        }
    }
}
