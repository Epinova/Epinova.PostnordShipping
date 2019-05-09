using System.Threading.Tasks;

namespace Epinova.PostnordShipping
{
    public interface IJsonFileService
    {
        Task<string> GetAllServicePointsRawAsync(ClientInfo clientInfo);
        Task<ServicePointInformation[]> LoadAllServicePointsAsync(ClientInfo clientInfo, bool forceCacheRefresh = false);
        Task<bool> SaveAllServicePointsRawAsync(ClientInfo clientInfo, string rawContent);
    }
}