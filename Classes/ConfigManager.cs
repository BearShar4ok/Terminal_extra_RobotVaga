using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RobotChanger.Classes
{
    public static class ConfigManager
    {
        private const string Path = "files/Config.json";

        public static Config Config { get; private set; }

        public static event Action Loaded
        {
            add => _manager.Loaded += value;
            remove => _manager.Loaded -= value;
        }

        public static event Action Saved
        {
            add => _manager.Saved += value;
            remove => _manager.Saved -= value;
        }

        private static Manager<Config> _manager;

        static ConfigManager() => _manager = new Manager<Config>(Path);

        public static void Load() => Config = _manager.Load();

        public static void Save() => _manager.Save(Config);
    }
}
