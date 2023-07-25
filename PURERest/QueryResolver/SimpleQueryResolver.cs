using System.Collections.Generic;

namespace PureREST.QueryResolver
{
    internal class SimpleQueryResolver : QueryResolverBase
    {
        public override Dictionary<string, string> resolve(string query)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            query = query.Replace("?", "");
            query = query.Replace("%20", " ");
            query = query.Replace("%C3%B6", "ö");
            query = query.Replace("%C3%BC", "ü");
            query = query.Replace("%C3%A4", "ä");


            var pairs = query.Split('&');

            foreach (string pair in pairs)
            {
                string name = pair.Split('=')[0];
                string value = pair.Replace(name + "=", "").Trim();

                value = value.Replace("+", " ");

                if (value.Contains("%22"))
                {
                    value = value.Replace("%22", "");

                }

                param.Add(name, value);
            }

            return param;
        }
    }
}
