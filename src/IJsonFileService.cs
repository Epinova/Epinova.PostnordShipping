using System.Threading.Tasks;

namespace Epinova.PostnordShipping
{
    public interface IJsonFileService
    {
        string GetAllServicePointsRaw(ClientInfo clientInfo);
        Task<ServicePointInformation[]> LoadAllServicePointsAsync(string filePath, bool forceCacheRefresh = false);
        bool SaveAllServicePointsRaw(string filePath, string rawContent);
    }
}