﻿using System;
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
    if (config is null)
        Console.WriteLine("Config is empty");
    else
        goto pass;
}
catch (IOException)
{
    Console.WriteLine("File is currently handled by another application");
}
catch (JsonException)
{
    Console.WriteLine("The file architecture is invalid");
}
return;

pass:
Handler handler = new Handler(config);