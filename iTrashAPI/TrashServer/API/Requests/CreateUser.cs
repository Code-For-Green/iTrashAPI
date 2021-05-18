using System.Text.Json;
using System.Threading.Tasks;

namespace TrashServer.API.Requests
{
    [RequestKey("create_user")]
    public class CreateUser : IRequest
    {
        public Task<string> Execute(string json)
        {
            User user = JsonSerializer.Deserialize<User>(json).Hashed();
            UserExtended userExt = new() { ID = Database.UserList.Count + 1, Login = user.Login, Password = user.Password };
            Database.UserList.Add(userExt);
            return Task.FromResult("OK");
        }
    }
}
