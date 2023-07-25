using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PureREST.Keys
{

    public class KeyManager
    {




        internal Dictionary<string, HttpKey> keys = new Dictionary<string, HttpKey>();
        private int previouskeys = 0;

        public string path = "RESTkey.txt";


        public void addKey(HttpKey key)
        {
            keys.Add(key.key, key);
            Console.WriteLine("Key Added to List: " + key.key);

        }

        public async Task load()
        {
            keys.Clear();
            //ConsoleManager.NormalLog(File.ReadAllLines(path).Length + " potential keys found");
            foreach (string line in File.ReadAllLines(path))
            {
                if (line.Split(';').Length > 5)
                {
                    var key = HttpKey.FromString(line);
                    Console.WriteLine("Added key: " + key.key);
                    keys.Add(key.key, key);

                }
                //ConsoleManager.NormalLog(keys.Count + " keys found");
                previouskeys = keys.Count;

            }
        }
        public async Task save()
        {
            string s = "";
            int i = 0;
            foreach (var key in keys.Values)
            {
                i++;
                s += key.ToString() + "\n";
            }
            if (previouskeys > i)
            {
                //ConsoleManager.ErrorLog("Lost key! Previously: " + previouskeys + " Saved: " + i);
                previouskeys = i;
            }
            File.WriteAllText(path, s);
        }

        public void useKey(string key, float contingent)
        {
            if (keys.ContainsKey(key))
            {
                keys[key].useKey(contingent);
            }
        }

        public bool isPrivileged(string key, params int[] privileges)
        {
            try
            {
                key = key.Trim();
                // Console.Write("Checking " + key + "\t");
                if (!keys.ContainsKey(key))
                {
                    Console.WriteLine("Key " + key + " not found!");
                    return false;
                }
                //Console.WriteLine("Key found!");
                var httpkey = keys[key];

                foreach (int privilege in privileges)
                {

                    if (!httpkey.isPrivileged(privilege))
                    {
                        Console.WriteLine("\tAccess not granted!");
                        return false;
                    }
                }
                Console.WriteLine("\tAccess granted!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }


    }



    public class HttpKey
    {
        public string key = "0000000000000000";
        public int ratelimit_min = 20;
        public int ratelimit_sec = 3;

        public float contingent = 100;

        public ulong owner_DC_ID = 0;

        public string comments = "";

        public DateTime created = DateTime.Now;
        public DateTime expired = DateTime.Now;

        public bool infinite_requests = false;

        public string privileges = "00";
        private List<long> request_timestamps = new List<long>();

        public void useKey(float contingent)
        {
            this.contingent -= contingent;
            request_timestamps.Add(DateTime.Now.Ticks);
        }

        public bool isPrivileged(int index)
        {


            if (privileges[0] == '1')
            {
                return true;
            }

            if (DateTime.Now > expired)
            {
                return false;
            }

            if (DateTime.Now < created)
            {
                return false;
            }

            if (isRatelimited())
            {
                return false;
            }

            if (contingent < 0 & !infinite_requests)
            {
                return false;
            }

            try
            {
                if (privileges[index] == '1')
                {
                    return true;
                }
            }
            catch (IndexOutOfRangeException)
            {
                privileges += "0";
                return false;
            }
            return false;
        }

        /// <summary>
        /// Checks if too much is requested
        /// </summary>
        /// <returns></returns>
        public bool isRatelimited()
        {
            int min_count = 0;
            int sec_count = 0;
            foreach (long timestamp in request_timestamps)
            {
                if ((DateTime.Now - (new DateTime(timestamp))).Seconds > 60)
                {
                    request_timestamps.Remove(timestamp);
                }

                if ((DateTime.Now - (new DateTime(timestamp))).Seconds < 60)
                {
                    min_count++;
                }

                if ((DateTime.Now - (new DateTime(timestamp))).Seconds < 1)
                {
                    sec_count++;
                }
            }

            if (min_count < ratelimit_min & sec_count < ratelimit_sec)
            {
                return false;
            }

            return true;
        }

        public override string ToString()
        {
            string s = "";

            s += key + ";" +
                ratelimit_min
                + ";" + ratelimit_sec
                + ";" + contingent
                + ";" + owner_DC_ID
                + ";" + comments.Replace(';', ' ')
                + ";" + created.Ticks
                + ";" + expired.Ticks
                + ";" + infinite_requests
                + ";" + privileges;

            return s;
        }

        public static HttpKey FromString(string s)
        {
            var vars = s.Split(';');
            HttpKey key = new HttpKey();

            key.key = vars[0];
            key.ratelimit_min = int.Parse(vars[1]);
            key.ratelimit_sec = int.Parse(vars[2]);
            key.contingent = int.Parse(vars[3]);
            key.owner_DC_ID = ulong.Parse(vars[4]);
            key.comments = vars[5];
            key.created = new DateTime(long.Parse(vars[6]));
            key.expired = new DateTime(long.Parse(vars[7]));
            key.infinite_requests = Convert.ToBoolean(vars[8]);
            key.privileges = vars[9];


            return key;

        }

    }
}


