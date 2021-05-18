using System;
using System.Net;

namespace TrashServer
{
    public class RequestException : Exception
    {
        public readonly HttpStatusCode StatusCode;

        public RequestException(HttpStatusCode statusCode) : base(Enum.GetName(statusCode)) => StatusCode = statusCode;

    }
}
