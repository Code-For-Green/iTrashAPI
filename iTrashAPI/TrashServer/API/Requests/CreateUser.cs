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
            UserExtended userExt = new(Database.UserList.Count + 1, user.Login, user.Password, null);
            Database.UserList.Add(userExt);
            return Task.FromResult("OK");
        }
    }
}
