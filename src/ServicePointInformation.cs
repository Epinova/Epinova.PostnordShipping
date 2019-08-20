namespace Epinova.PostnordShipping
{
    public class ServicePointInformation
    {
        public ServicePointInformation()
        {
            OpeningHours = new OpeningHourInfo[0];
        }

        public string CoordinateId { get; set; }
        public AddressInfo DeliveryAddress { get; set; }
        public float Easting { get; set; }
        public string Id { get; set; }
        public bool IsEligibleParcelOutlet { get; set; }
        public string Name { get; set; }
        public float Northing { get; set; }
        public string[] NotificationPostalCodes { get; set; }
        public OpeningHourInfo[] OpeningHours { get; set; }
        public int RouteDistance { get; set; }
        public AddressInfo VisitingAddress { get; set; }
    }
}