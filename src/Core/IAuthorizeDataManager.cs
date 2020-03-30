using System.Threading.Tasks;

namespace AuthorizationExtension.Core
{
    public interface IAuthorizeDataManager
    {
        Task<AuthorizeData> GetAuthorizeDataAsync();
    }
}