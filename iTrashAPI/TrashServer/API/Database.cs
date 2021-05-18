using System;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;

namespace TrashServer.API
{
    public static class Database
    {
        private static string _userListPath;
        private static Thread _thread;

        public static ConcurrentBag<UserExtended> UserList { get; private set; }
        public static ConcurrentQueue<UserToken> ActiveUserList { get; private set; }

        public static void Init(string path)
        {
            ActiveUserList = new();
            _userListPath = Path.Combine(path, "users.json");
            _thread = new(new ThreadStart(DatabaseHandler)) { IsBackground = true, Name = "Database" };
            _thread.Start();
        }

        public static void Close()
        {
            _thread.Interrupt();
        }

        public static void Load()
        {
            if (File.Exists(_userListPath))
            {
                using StreamReader reader = new(File.OpenRead(_userListPath));
                IEnumerable<UserExtended> enumerable = JsonSerializer.Deserialize<List<UserExtended>>(reader.ReadToEnd());
                UserList = new(enumerable);
            }
            else
            {
                UserList = new();
            }
        }

        public static bool GetUser(string token, out User user)
        {
            UserToken userToken = ActiveUserList.First(loggedUser => loggedUser.Token == token);
            user = userToken?.ActiveUser;
            return user is not null;
        }

        public static void Save()
        {
            using StreamWriter writer = new(File.Create(_userListPath));
            List<UserExtended> users = new(UserList);
            writer.Write(JsonSerializer.Serialize(users, new JsonSerializerOptions() {WriteIndented = true}));
        }

        private static void DatabaseHandler()
        {
            try
            {
                while(true)
                {
                    if (!ActiveUserList.IsEmpty)
                        CheckExpiration();
                    Thread.Sleep(1000);
                }
            }
            catch(ThreadInterruptedException)
            {

            }
            finally
            {
            
            }
        }

        private static void CheckExpiration()
        {
            long time = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
            while (true)
            {
                if (ActiveUserList.IsEmpty)
                    return;
                if (!ActiveUserList.TryPeek(out UserToken user))
                    return;
                if (user.Expiration >= time)
                    return;
                if (!ActiveUserList.TryDequeue(out _))
                    return;
            }
        }
    }
}
