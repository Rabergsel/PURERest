using System.Collections.Generic;

namespace PureREST
{

    public class Diagnostics
    {
        public long Requests = 0;
        public Dictionary<string, long> requestsByEndpoint = new Dictionary<string, long>();

        public ulong workMS = 0;

    }

}
