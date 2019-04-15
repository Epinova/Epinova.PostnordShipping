using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Epinova.Infrastructure;
using Epinova.Infrastructure.Logging;
using EPiServer.Logging;
using Newtonsoft.Json;

namespace Epinova.PostnordShipping
{
    public class JsonFileService : RestServiceBase, IJsonFileService
    {
        internal static WebClient Client = new WebClient { BaseAddress = Constants.BaseUrl, Encoding = Encoding.UTF8 };
        private readonly ICacheHelper _cacheHelper;
        private readonly ILogger _log;
        private readonly IMapper _mapper;

        public JsonFileService(ILogger log, IMapper mapper, ICacheHelper cacheHelper) : base(log)
        {
            _log = log;
            _mapper = mapper;
            _cacheHelper = cacheHelper;
        }

        public override string ServiceName => nameof(JsonFileService);

        public string GetAllServicePointsRaw(ClientInfo clientInfo)
        {
            var parameters = new Dictionary<string, string>
            {
                { "apikey", clientInfo.ApiKey },
                { "countryCode", clientInfo.Country.ToString() }
            };

            string url = $"businesslocation/v1/servicepoint/getServicePointInformation.json?{BuildQueryString(parameters)}";
            string content = Client.DownloadString(url);

            if (String.IsNullOrWhiteSpace(content))
            {
                _log.Error(new { message = "Find All Service Points Async failed. Service response was NULL" });
                return null;
            }

            return content;
        }

        public async Task<ServicePointInformation[]> LoadAllServicePointsAsync(string filePath, bool forceCacheRefresh = false)
        {
            const string cacheKey = "servicepointsfallback";
            ServicePointInformation[] result;

            if (!forceCacheRefresh)
            {
                result = _cacheHelper.Get<ServicePointInformation[]>(cacheKey);
                if (result != null)
                    return result;
            }

            ServicePointInformationRootDto dto = await ReadFromJsonFileAsync<ServicePointInformationRootDto>(filePath);
            if (dto?.ServicePointInformationResponse?.ServicePoints == null)
            {
                _log.Critical("unable to read service points from disk");
                return new ServicePointInformation[0];
            }

            result = _mapper.Map<ServicePointInformation[]>(dto.ServicePointInformationResponse.ServicePoints.Where(x => x.EligibleParcelOutlet)).Distinct(new ServicePointInformationComparer())
                .ToArray();

            _cacheHelper.Insert(cacheKey, result, TimeSpan.FromDays(2));
            return result;
        }

        public bool SaveAllServicePointsRaw(string filePath, string rawContent)
        {
            try
            {
                JsonConvert.DeserializeObject<ServicePointInformationRootDto>(rawContent);
            }
            catch (Exception ex)
            {
                _log.Critical("Unable to serialize content as DTO. Postnord pickup point list not updated.", ex);
                return false;
            }

            return WriteToJsonFile(filePath, rawContent);
        }

        private static async Task<T> ReadFromJsonFileAsync<T>(string filePath) where T : new()
        {
            TextReader reader = null;
            try
            {
                reader = new StreamReader(filePath, Encoding.UTF8);
                string fileContents = await reader.ReadToEndAsync();
                return JsonConvert.DeserializeObject<T>(fileContents);
            }
            finally
            {
                reader?.Close();
            }
        }

        private bool WriteToJsonFile(string filePath, string contentToWrite)
        {
            bool fileSaved;

            TextWriter writer = null;
            try
            {
                writer = new StreamWriter(filePath, false, Encoding.UTF8);
                writer.Write(contentToWrite);
                fileSaved = true;
            }
            catch (Exception e)
            {
                _log.Error(e);
                fileSaved = false;
            }
            finally
            {
                writer?.Close();
            }

            return fileSaved;
        }
    }
}