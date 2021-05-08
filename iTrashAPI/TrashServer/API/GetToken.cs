using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TrashServer.API
{
    [RequestKey("token")]
    public class GetToken : IRequest
    {
        public Task Execute(in JsonElement json, out string response)
        {
            response = string.Empty;
            return Task.CompletedTask;
        }
    }
}
