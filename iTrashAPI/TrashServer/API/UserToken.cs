using System.Text.Json.Serialization;

namespace TrashServer.API
{
    public record UserToken
    {
        public string Token { get; init; }
        public long Expiration { get; init; }
        [JsonIgnore]
        public UserExtended ActiveUser { get; init; }
    }
}
