namespace TrashServer.API
{
    public record UserToken
    {
        public string Token { get; init; }
        public long Expiration { get; init; }
        public User ActiveUser { get; init; }
    }
}
