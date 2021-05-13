namespace TrashServer
{
    public class Config
    {
        public string EndPoint { get; set; } = "http://localhost/";
        public LogLevel LogLevel { get; set; } = LogLevel.Info;
        public string DatabasePath { get; set; } = string.Empty;
    }
}
