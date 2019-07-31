using AutoMapper;
using StructureMap;

namespace Epinova.PostnordShipping
{
    public class DeliveryRegistry : Registry
    {
        public DeliveryRegistry()
        {
            var mapperConfiguration = new MapperConfiguration(cfg => { cfg.AddProfile(new DeliveryMappingProfile()); });
            mapperConfiguration.CompileMappings();

            For<IDeliveryService>().Use<DeliveryService>().Ctor<IMapper>().Is(mapperConfiguration.CreateMapper());
            For<ICacheHelper>().Use<CacheHelper>();
        }
    }
}