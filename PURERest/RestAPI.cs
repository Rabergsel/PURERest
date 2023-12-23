using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace PureREST
{
    public class RESTAPI
    {

        private HttpListener listener = new HttpListener();
        private Dictionary<string, APIAction> actions = new Dictionary<string, APIAction>();

        private QueryResolver.QueryResolverBase resolver = new QueryResolver.SimpleQueryResolver();
        private Keys.KeyManager keymanager = new Keys.KeyManager();



        public RESTAPI(string folder)
        {
            keymanager.path = "./" + folder + "/keys.txt";
        }

        /// <summary>
        /// Registeres a URL for the REST API
        /// </summary>
        /// <param name="urls"></param>
        public void registerURL(params string[] urls)
        {
            foreach (string url in urls)
            {
                listener.Prefixes.Add(url);
            }
        }

        public void addKey(Keys.HttpKey key)
        {
            keymanager.addKey(key);
            save();
        }

        public bool doesKeyExist(string key)
        {
            return keymanager.keys.ContainsKey(key);
        }

        public void addAPIEndPoint(string actionURL, Func<Dictionary<string, string>, string> action, params int[] permissions)
        {
            if (!actionURL.StartsWith('/'))
            {
                actionURL = "/" + actionURL;
            }

            var act = new APIAction();
            act.action = action;
            foreach (int i in permissions)
            {
                act.permissions.Add(i);
            }
            actions.Add(actionURL, act);
            diagnostics.requestsByEndpoint.Add(actionURL, 0);



        }

        public void addKey(string key, int requestsPerSec, int requestsPerMin, float contingent, string permissions, string comment = "", int secondsValid = -1)
        {
            var k = new Keys.HttpKey()
            {
                key = key,
                created = DateTime.Now,
                ratelimit_sec = requestsPerSec,
                ratelimit_min = requestsPerMin,
                contingent = contingent,
                privileges = permissions,
                comments = comment
            };

            if(secondsValid != -1)
            {
                k.expired = k.created + TimeSpan.FromSeconds(secondsValid);
            }

            keymanager.addKey(k);

            Console.WriteLine("added key: " + key);
            save();
        }

        public bool checkPermissions(int[] permissions, string apikey)
        {
            return keymanager.isPrivileged(apikey, permissions);
        }

        public Diagnostics diagnostics = new Diagnostics();

        public void save()
        {
            keymanager.save();
        }

        public async void Start()
        {
            await keymanager.load();
            Console.WriteLine("Starting API with those keys: ");
            foreach (var key in keymanager.keys.Values)
            {
                Console.WriteLine(key.key);
            }


            listener.Start();
            while (true)
            {
                // Console.WriteLine("Loop!");
                HttpListenerContext ctx = listener.GetContext();
                using HttpListenerResponse resp = ctx.Response;

                var stopwatch = System.Diagnostics.Stopwatch.StartNew();

                var url = ctx.Request.Url.AbsoluteUri;
                var querystring = ctx.Request.Url.Query;
                var query = resolver.resolve(querystring);
                resp.StatusCode = (int)HttpStatusCode.OK;

                string answer = "";

                string actionurl = url.Replace("http://", "");
                diagnostics.Requests++;
                actionurl = actionurl.Replace(actionurl.Split('/')[0], "");
                try
                {
                    actionurl = actionurl.Split('?')[0];
                }
                catch
                {

                }
                Console.WriteLine("Action URL: " + actionurl);

                foreach (var action in actions)
                {
                    //Console.WriteLine("Checking: " + action.Key);
                    if (action.Key == actionurl)
                    {
                        if(!query.ContainsKey("apikey"))
                        {
                            break;
                        }
                        if (!checkPermissions(action.Value.permissions.ToArray(), query["apikey"]))
                        {
                            break;
                        }
                        //Console.WriteLine("Found!");
                        answer = action.Value.action.Invoke(query);
                        Console.WriteLine(answer);
                        diagnostics.requestsByEndpoint[action.Key]++;
                        break;
                    }
                }

                string data = answer;
                byte[] buffer = Encoding.UTF8.GetBytes(data);
                resp.ContentLength64 = buffer.Length;

                using Stream ros = resp.OutputStream;
                ros.Write(buffer, 0, buffer.Length);
                stopwatch.Stop();
                diagnostics.workMS += (ulong)stopwatch.ElapsedMilliseconds;

            }
        }

    }
}
