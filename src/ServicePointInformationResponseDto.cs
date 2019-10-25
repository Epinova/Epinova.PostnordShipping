namespace Epinova.PostnordShipping
{
    internal class ServicePointInformationResponseDto
    {
        public CompositeFaultDto CompositeFault { get; set; }
        public CoordinateDto Coordinate { get; set; }
        public string CustomerSupportPhoneNo { get; set; }
        public ServicePointDto[] ServicePoints { get; set; }
    }
}
