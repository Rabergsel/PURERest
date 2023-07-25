using System;
using System.Collections.Generic;

namespace PureREST
{
    public class APIAction
    {
        public Func<Dictionary<string, string>, string> action;
        public List<int> permissions = new List<int>();
    }
}
