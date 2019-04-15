using System.Linq;

namespace Epinova.PostnordShipping
{
    internal class ServicePointDto
    {
        public ServicePointDto()
        {
            Coordinates = new CoordinateDto[0];
            OpeningHours = new OpeningHourDto[0];
        }

        /// <remarks>Use <see cref="GetCoordinate"/> instead</remarks>
        public CoordinateDto Coordinate { get; set; }

        /// <remarks>Use <see cref="GetCoordinate"/> instead</remarks>
        public CoordinateDto[] Coordinates { get; set; }

        public AddressDto DeliveryAddress { get; set; }

        public bool EligibleParcelOutlet { get; set; }
        public string LocationDetail { get; set; }
        public string Name { get; set; }
        public NotificationAreaDto NotificationArea { get; set; }
        public OpeningHourDto[] OpeningHours { get; set; }
        public int RouteDistance { get; set; }
        public string RoutingCode { get; set; }
        public string ServicePointId { get; set; }
        public AddressDto VisitingAddress { get; set; }

        public CoordinateDto GetCoordinate()
        {
            return Coordinate ?? Coordinates?.FirstOrDefault() ?? new CoordinateDto();
        }
    }
}