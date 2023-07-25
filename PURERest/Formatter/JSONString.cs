namespace PureREST
{
    public class JSONString
    {


        private string json = "";


        public void addValue(string name, string value)
        {
            json += "\"" + name + "\":\"" + value + "\",\n";
        }

        public void addValue(string name, JSONString value)
        {
            json += "\"" + name + "\":" + value.ToString() + ",\n";
        }

        public void addValue(string name, int value)
        {
            json += "\"" + name + "\": " + value + ",\n";
        }
        public void addValue(string name, ulong value)
        {
            json += "\"" + name + "\": " + value + ",\n";
        }
        public void addValue(string name, long value)
        {
            json += "\"" + name + "\": " + value + ",\n";
        }

        public void addValue(string name, float value)
        {
            json += "\"" + name + "\": " + value + ",\n";
        }

        public override string ToString()
        {
            try
            {
                return "{\n" + json.Remove(json.Length - 2) + "\n}";
            }
            catch
            {
                return "{}";
            }
        }
    }
}
