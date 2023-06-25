using Microsoft.Extensions.Configuration;
using Test;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", false, true)
    .Build();


const string serverName = "Default";
const string javaVersion = "17";
var serverDirectory = config["ServersFolder"];
if (string.IsNullOrEmpty(serverDirectory) || !Directory.Exists(serverDirectory))
{
    Console.WriteLine("Servers directory not found!");
    return;
}



Console.WriteLine("Creating server container...");

// Maybe recreate as builder to keep track of states and avoid errors ???
var server = new MinecraftServer(await ServerManager.CreateContainer(serverDirectory, serverName, javaVersion));

Console.WriteLine("Starting server container...");

await ServerManager.StartServerContainer(serverName);
Console.WriteLine("Done!");



var logs = server.GetContainerLogsAsync();
while (true)
{
    var input = Console.ReadLine();
    if (string.IsNullOrEmpty(input)) continue;
    await server.ExecCommandAsync(input);
    if (input == "stop")
    {
        Console.WriteLine("Stopping server container...");
        break;
    }
}
logs.Dispose();