using System.Text.Json;
using System.Threading.Tasks;

namespace TrashServer.API.Requests
{
    [RequestKey("create_user")]
    public class CreateUser : IRequest
    {
        public Task Execute(string json, out string response)
        {
            User user = JsonSerializer.Deserialize<User>(json);
            User hashedUser = user.Hashed();
            Database.UserList.Add(hashedUser);
            response = "OK";
            return Task.CompletedTask;
        }
    }
}
