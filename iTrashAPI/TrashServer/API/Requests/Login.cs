using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace TrashServer.API.Requests
{
    [RequestKey("login")]
    public class Login : IRequest
    {
        private readonly Random _random = new();
        public Task Execute(string json, out string response)
        {
            User user = JsonSerializer.Deserialize<User>(json);
            User hashedUser = user.Hashed();
            if (!Database.UserList.Contains(hashedUser))
                throw new Exception("Wrong!");
            DateTime foo = DateTime.UtcNow.AddMinutes(10);
            long unixTime = ((DateTimeOffset)foo).ToUnixTimeSeconds();
            UserToken userToken = new UserToken { Token = GenerateToken(), Expiration = unixTime };
            response = JsonSerializer.Serialize(userToken);
            return Task.CompletedTask;
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
