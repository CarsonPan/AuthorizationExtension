using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace AuthorizationExtension.Core
{
    public interface IPolicyCombiner
    {
        Task<AuthorizationPolicy> CombineAsync(AuthorizeData authorizeData);
    }
}