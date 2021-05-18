using System;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;

namespace TrashServer.API
{
    public static class Database
    {
        private static string _userListPath;
        private static List<User> _userList;
        private static ConcurrentQueue<User> _activeUserList;

        public static IList<User> UserList => _userList;
        public static ConcurrentQueue<User> ActiveUserList => _activeUserList;

        public static void Init(string path)
        {
            _userListPath = Path.Combine(path, "users.json");
            _activeUserList = new();
        }

        public static void Load()
        {
            if (File.Exists(_userListPath))
            {
                using StreamReader reader = new(File.OpenRead(_userListPath));
                _userList = JsonSerializer.Deserialize<List<User>>(reader.ReadToEnd());
            }
            else
            {
                _userList = new();
            }
        }

        public static void Save()
        {
            using StreamWriter writer = new(File.Create(_userListPath));
            writer.Write(JsonSerializer.Serialize(_userList));
        }
    }
}
