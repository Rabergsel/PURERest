using System.Collections.Generic;

namespace PureREST.QueryResolver
{
    internal abstract class QueryResolverBase
    {
        public abstract Dictionary<string, string> resolve(string query);

    }
}
