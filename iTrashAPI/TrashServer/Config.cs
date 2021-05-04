namespace TrashServer
{
    public class Config
    {
        public string EndPoint { get; private set; } = "/relay/";
        public ushort Port { get; private set; } = 8080;
        public LogLevel LogLevel { get; private set; } = LogLevel.Info;
    }
}
