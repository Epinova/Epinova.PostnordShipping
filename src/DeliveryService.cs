using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Epinova.Infrastructure;
using Epinova.Infrastructure.Logging;
using EPiServer.Logging;

namespace Epinova.PostnordShipping
{
    internal class DeliveryService : RestServiceBase, IDeliveryService
    {
        internal static HttpClient Client;
        private readonly ICacheHelper _cacheHelper;
        private readonly IJsonFileService _fileService;
        private readonly ILogger _log;
        private readonly IMapper _mapper;

        static DeliveryService()
        {
            Client = new HttpClient { BaseAddress = new Uri(Constants.BaseUrl), Timeout = ApplicationSettings.TimeOut };
        }

        public DeliveryService(IJsonFileService fileService, ILogger log, IMapper mapper, ICacheHelper cacheHelper) : base(log)
        {
            _fileService = fileService;
            _log = log;
            _mapper = mapper;
            _cacheHelper = cacheHelper;
        }

        public override string ServiceName => nameof(DeliveryService);

        public async Task<ServicePointInformation[]> FindServicePointsAsync(ClientInfo client, double latitude, double longitude, int maxResults)
        {
            return (await _fileService.LoadAllServicePointsAsync(client.FilePath))
                .Select(x => new { ServicePoint = x, Distance = GetDistanceFromLatLonInKm(latitude, longitude, x.Northing, x.Easting) })
                .OrderBy(x => x.Distance)
                .Select(x => x.ServicePoint)
                .Take(maxResults)
                .ToArray();
        }

        public double GetDistanceFromLatLonInKm(double lat1, double lon1, double lat2, double lon2)
        {
            double Deg2Rad(double deg) => deg * (Math.PI / 180);

            var R = 6371; // Radius of the earth in km
            double dLat = Deg2Rad(lat2 - lat1); // deg2rad below
            double dLon = Deg2Rad(lon2 - lon1);
            double a =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(Deg2Rad(lat1)) * Math.Cos(Deg2Rad(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c; // Distance in km
        }

        public async Task<ServicePointInformation> GetServicePointAsync(ClientInfo clientInfo, string pickupPointId, bool forceCacheRefresh)
        {
            if (String.IsNullOrWhiteSpace(pickupPointId))
            {
                _log.Warning(new { message = "Invalid service point fetch request", pickupPointId });
                return null;
            }

            string cacheKey = $"ServicePoint_{pickupPointId}";

            ServicePointInformation result =null;
            if (!forceCacheRefresh)
            {
                result = _cacheHelper.Get<ServicePointInformation>(cacheKey);
                if (result != null)
                    return result;

                result = (await _fileService.LoadAllServicePointsAsync(clientInfo.FilePath)).FirstOrDefault(x => x.Id == pickupPointId);
            }

            if (result == null)
            {
                _log.Information($"Pickup point {pickupPointId} not found in application cache. Getting it directly from Postnord");
                result = await GetServicePointLiveAsync(clientInfo, pickupPointId);
            }

            _cacheHelper.Insert(cacheKey, result, TimeSpan.FromDays(2));
            return result;
        }

        internal async Task<ServicePointInformation> GetServicePointLiveAsync(ClientInfo clientInfo, string pickupPointId)
        {
            var parameters = new Dictionary<string, string>
            {
                { "apikey", clientInfo.ApiKey },
                { "countryCode", clientInfo.Country.ToString() },
                { "servicePointId", pickupPointId },
            };

            string url = $"businesslocation/v1/servicepoint/findByServicePointId.json?{BuildQueryString(parameters)}";

            HttpResponseMessage responseMessage = await Call(() => Client.GetAsync(url), true);

            if (responseMessage == null)
            {
                _log.Error(new { message = "Service point fetch failed. Service response was NULL", pickupPointId });
                return null;
            }

            ServicePointInformationRootDto dto = await ParseJson<ServicePointInformationRootDto>(responseMessage);

            if (dto.HasError || dto.ServicePointInformationResponse?.ServicePoints == null)
            {
                _log.Error(new { message = "Service point fetch failed.", pickupPointId, dto.ErrorMessage });
                return null;
            }

            return _mapper.Map<ServicePointInformation[]>(dto.ServicePointInformationResponse.ServicePoints).FirstOrDefault();
        }
    }
}