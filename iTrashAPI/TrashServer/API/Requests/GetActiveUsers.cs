using System.Text.Json;
using System.Threading.Tasks;

namespace TrashServer.API.Requests
{
    [RequestKey("get_users")]
    public class GetActiveUsers : IRequest
    {
        public Task<string> Execute(string json)
        {
            if (Database.ActiveUserList.IsEmpty)
                return Task.FromResult("{ }");
            return Task.FromResult(JsonSerializer.Serialize(Database.ActiveUserList));
        }
    }
}
