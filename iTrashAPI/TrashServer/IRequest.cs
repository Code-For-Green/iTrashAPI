using System.Text.Json;
using System.Threading.Tasks;

namespace TrashServer
{
    public interface IRequest
    {
        public Task Execute(string json, out string response);
    }
}
