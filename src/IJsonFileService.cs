using System.Threading.Tasks;

namespace Epinova.PostnordShipping
{
    public interface IJsonFileService
    {
        string GetAllServicePointsRaw(ClientInfo clientInfo);
        Task<ServicePointInformation[]> LoadAllServicePointsAsync(ClientInfo clientInfo, bool forceCacheRefresh = false);
        bool SaveAllServicePointsRaw(ClientInfo clientInfo, string rawContent);
    }
}