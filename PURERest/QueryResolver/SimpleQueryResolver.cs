using System.Collections.Generic;

namespace PureREST.QueryResolver
{
    internal class SimpleQueryResolver : QueryResolverBase
    {

        Dictionary<string, string> mappings = new Dictionary<string, string>(
            new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("%20", " "),
                new KeyValuePair<string, string>("C3%B6", "ö"),
                new KeyValuePair<string, string>("C3%BC", "ü"),
                new KeyValuePair<string, string>("C3%A4", "ä"),
                new KeyValuePair<string, string>("%21", "!"),
                new KeyValuePair<string, string>("%22", "\""),
                new KeyValuePair<string, string>("%23", "#"),
                new KeyValuePair<string, string>("%24", "$"),
                new KeyValuePair<string, string>("%25", "%"),
                new KeyValuePair<string, string>("%26", "&"),
                new KeyValuePair<string, string>("%27", "'"),
                new KeyValuePair<string, string>("%28", "("),
                new KeyValuePair<string, string>("%29", ")"),
                new KeyValuePair<string, string>("%2A", "*"),
                new KeyValuePair<string, string>("%2B", "+"),
                new KeyValuePair<string, string>("%2C", ","),
                new KeyValuePair<string, string>("%2F", "/"),
                new KeyValuePair<string, string>("%3A", ":"),
                new KeyValuePair<string, string>("%3B", ";"),
                new KeyValuePair<string, string>("%3C", "<"),
                new KeyValuePair<string, string>("%3D", "="),
                new KeyValuePair<string, string>("%3E", ">"),
                new KeyValuePair<string, string>("%3F", "?"),
                new KeyValuePair<string, string>("%40", "@"),
                new KeyValuePair<string, string>("%5B", "["),
                new KeyValuePair<string, string>("%5C", "\\"),
                new KeyValuePair<string, string>("%5D", "]"),
                new KeyValuePair<string, string>("%5E", "^"),
                new KeyValuePair<string, string>("%5F", "_"),
                new KeyValuePair<string, string>("%60", "`"),
                new KeyValuePair<string, string>("%7B", "{"),
                new KeyValuePair<string, string>("%7C", "|"),
                new KeyValuePair<string, string>("%7D", "}"),
                new KeyValuePair<string, string>("%7E", "~")

            });

        public override Dictionary<string, string> resolve(string query)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            query = query.Replace("?", "");

            foreach(KeyValuePair<string, string> kvp in mappings)
            {
                query = query.Replace(kvp.Key, kvp.Value);
            }
            


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
