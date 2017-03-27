using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.UWP.Library
{
    public class QueryReader
    {
        Dictionary<string, string> _values;

        public string this[string name]
        {
            get
            {
                if (_values.ContainsKey(name))
                    return _values[name];
                else
                    return null;
            }
        }

        public bool Contains(string key)
        {
            return _values.ContainsKey(key.ToLower());
        }

        public static QueryReader Load(string query_string)
        {
            var reader = new QueryReader();
            reader._values = new Dictionary<string, string>();
            if (query_string.StartsWith("?"))
            {
                query_string = query_string.TrimStart('?');
                var parts = query_string.ToLower().Split(',');
                foreach (var query_item in parts)
                {
                    var key_value = query_item.Split('=');
                    if (key_value.Length == 2)
                        reader._values.Add(key_value[0], key_value[1]);
                }
            }
            return reader;
        }
    }
}
