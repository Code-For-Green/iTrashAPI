using System;
using System.Net;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace TrashServer.API.Requests
{
    [RequestKey("login")]
    public class Login : IRequest
    {
        private readonly Random _random = new();
        private readonly long roundDownTime = TimeSpan.FromMinutes(1).Ticks;

        public Task<string> Execute(string json)
        {
            User user = JsonSerializer.Deserialize<User>(json).Hashed();
            if (!Database.UserList.Any(x=>x.Login == user.Login && x.Password == user.Password))
                throw new RequestException(HttpStatusCode.Unauthorized);
            DateTime time = DateTime.UtcNow.AddMinutes(10);
            time = new DateTime(time.Ticks - (time.Ticks % roundDownTime), time.Kind);
            long unixTime = ((DateTimeOffset)time).ToUnixTimeSeconds();
            UserToken userToken = new() { Token = GenerateToken(), Expiration = unixTime, ActiveUser = Database.UserList.First(userExt=> userExt.Login == user.Login)};
            Database.ActiveUserList.Enqueue(userToken);
            return Task.FromResult(JsonSerializer.Serialize(userToken));
        }

        private string GenerateToken()
        {
            byte[] result = new byte[8];
            for(int i = 0; i < 8; i++ )
                 result[i] = (byte)_random.Next(byte.MaxValue);
            return Convert.ToBase64String(result);
        }
    }
}
