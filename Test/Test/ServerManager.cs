using Docker.DotNet;
using Docker.DotNet.Models;

namespace Test;

public static class ServerManager
{
    private static readonly DockerClient DockerClient =
        new DockerClientConfiguration(new Uri("npipe://./pipe/docker_engine")).CreateClient();

    public static async Task<CreateContainerResponse> CreateContainer(string serversDirectory, string serverName,
        string javaVersion)
    {
        var serverPath = $"{serversDirectory}/{serverName}";
        var serverImage = $"amazoncorretto:{javaVersion}";

        var parameters = new CreateContainerParameters
        {
            Image = serverImage,
            Name = serverName,
            HostConfig = new HostConfig
            {
                Binds = new List<string>
                {
                    $"{serverPath.Replace("\\", "/")}:/app"
                }
            },
            WorkingDir = "/app/",
            Cmd = new List<string> {"python", "server_mediator.py"},
            AttachStdout = true,
            AttachStdin = true,
            Tty = true
        };

        return await DockerClient.Containers.CreateContainerAsync(parameters);
    }

    public static async Task StartServerContainer(string serverName)
    {
        await DockerClient.Containers.StartContainerAsync(serverName, new ContainerStartParameters());
    }

    public static async Task<bool> CheckServerContainerExists(string serverName)
    {
        var containers = await DockerClient.Containers.ListContainersAsync(new ContainersListParameters());
        return containers.Any(container => container.Names.Contains($"/{serverName}"));
    }
    
    public static async Task<bool> CheckServerContainerRunning(string serverName)
    {
        var containers = await DockerClient.Containers.ListContainersAsync(new ContainersListParameters());
        return containers.Any(container => container.Names.Contains($"/{serverName}") && container.State == "running");
    }
}