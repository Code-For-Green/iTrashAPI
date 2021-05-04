namespace TrashServer
{
    public class Config
    {
        public string EndPoint { get; private set; } = "/relay/";
        public ushort Port { get; private set; } = 8080;
    }
}
