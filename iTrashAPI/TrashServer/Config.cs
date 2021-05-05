namespace TrashServer
{
    public class Config
    {
        public string EndPoint { get; set; } = "http://localhost/";
        public ushort Port { get; set; } = 8080;
        public LogLevel LogLevel { get; set; } = LogLevel.Info;
    }
}
