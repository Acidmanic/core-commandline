using System.Collections.Generic;

namespace CoreCommandLine
{
    public class Context
    {
        internal CommandUtilities CommandUtilities { get; }
        
        private readonly Dictionary<string, object> _data = new();

        public bool IsApplicationRunningInteractive { get;  }

        internal Context(CommandUtilities commandUtilities, bool isApplicationRunningInteractive)
        {
            CommandUtilities = commandUtilities;
            
            IsApplicationRunningInteractive = isApplicationRunningInteractive;
        }


        public void Set<T>(string key, T value)
        {
            if (_data.ContainsKey(key))
            {
                _data.Remove(key);
            }

            _data.Add(key, value);
        }

        public T Get<T>(string key, T defaultValue)
        {
            if (_data.ContainsKey(key))
            {
                return (T)_data[key];
            }

            return defaultValue;
        }

        public bool Contains(string key)
        {
            return _data.ContainsKey(key);
        }

        public bool ApplicationExit { get; set; } = false;
        public bool InteractiveExit { get; set; } = false;
    }
}