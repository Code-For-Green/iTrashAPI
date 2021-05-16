using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace TrashServer.API.Requests
{
    [RequestKey("login")]
    public class Login : IRequest
    {
        private readonly Random _random = new();
        public Task<string> Execute(string json)
        {
            User user = JsonSerializer.Deserialize<User>(json);
            User hashedUser = user.Hashed();
            if (!Database.UserList.Contains(hashedUser))
                throw new Exception("Wrong!");
            DateTime foo = DateTime.UtcNow.AddMinutes(10);
            long unixTime = ((DateTimeOffset)foo).ToUnixTimeSeconds();
            UserToken userToken = new() { Token = GenerateToken(), Expiration = unixTime };
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
