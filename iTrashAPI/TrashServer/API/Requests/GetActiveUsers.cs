using System.Text.Json;
using System.Threading.Tasks;

namespace TrashServer.API.Requests
{
    [RequestKey("get_users")]
    public class GetActiveUsers : IRequest
    {
        public Task<string> Execute(string json)
        {
            User user = JsonSerializer.Deserialize<User>(json);

            return Task.FromResult("OK");
        }
    }
}
