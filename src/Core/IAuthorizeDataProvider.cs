using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace AuthorizationExtension.Core
{
    public interface IAuthorizeDataProvider
    {
        Task<AuthorizeData[]> GetAuthorizeDatasAsync(string resourceId,CancellationToken cancellationToken);
    }
}