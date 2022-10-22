using System.Collections.Generic;

namespace CoreCommandLine
{
    public class Context
    {
        private Dictionary<string, object> _data = new Dictionary<string, object>();

        public void Set<T>(string key, T value)
        {
            if (_data.ContainsKey(key))
            {
                _data.Remove(key);
            }
            _data.Add(key,value);
        }

        public T Get<T>(string key) where T : class
        {
            if (_data.ContainsKey(key))
            {
                return _data[key] as T;
            }

            return default;
        }

        public bool Contains(string key)
        {
            return _data.ContainsKey(key);
        }
    }
}