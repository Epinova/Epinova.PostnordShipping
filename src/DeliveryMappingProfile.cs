using AutoMapper;

namespace Epinova.PostnordShipping
{
    internal class DeliveryMappingProfile : Profile
    {
        public DeliveryMappingProfile()
        {
            AllowNullCollections = false;
            CreateMap<ServicePointDto, ServicePointInformation>()
                .ForMember(dest => dest.Easting, opt => opt.MapFrom(src => src.GetCoordinate().Easting))
                .ForMember(dest => dest.Northing, opt => opt.MapFrom(src => src.GetCoordinate().Northing))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ServicePointId))
                .ForMember(dest => dest.CoordinateId, opt => opt.MapFrom(src => src.GetCoordinate().Id))
                .ForMember(dest => dest.NotificationPostalCodes, opt => opt.MapFrom(src => src.NotificationArea.PostalCodes))
                .ForMember(dest => dest.IsEligibleParcelOutlet, opt => opt.MapFrom(src => src.EligibleParcelOutlet));
            CreateMap<OpeningHourDto, OpeningHourInfo>();
            CreateMap<AddressDto, AddressInfo>();
        }
    }
}
