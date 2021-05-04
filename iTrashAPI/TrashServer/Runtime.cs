using System;
using System.IO;
using System.Text.Json;
using TrashServer;

const string path = "config.json";
Config config;

//Checking config
if (!File.Exists(path))
{
    File.WriteAllText(path, JsonSerializer.Serialize<Config>(new()));
    return;
}
try
{
    config = JsonSerializer.Deserialize<Config>(File.ReadAllText(path));
}
catch (IOException)
{
    Console.WriteLine("File is currently handled by another application");
    return;
}
catch (JsonException)
{
    Console.WriteLine("The file architecture is invalid");
    return;
}
if (config is null)
{
    Console.WriteLine("Config is empty");
    return;
}