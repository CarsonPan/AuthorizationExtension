using System.Collections.Generic;

namespace AuthorizationExtension.Core
{
    public interface IRoleAccessor
    {
        IList<string> Roles{get;}
    }
}