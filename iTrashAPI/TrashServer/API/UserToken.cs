using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrashServer.API
{
    public record UserToken
    {
        public string Token { get; init; }
        public long Expiration { get; init; }
    }
}
