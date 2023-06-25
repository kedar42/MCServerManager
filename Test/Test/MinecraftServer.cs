namespace Test;

using System.Text;
using Docker.DotNet;
using Docker.DotNet.Models;

public class MinecraftServer
{
    private readonly DockerClient _dockerClient =
        new DockerClientConfiguration(new Uri("npipe://./pipe/docker_engine")).CreateClient();

    private readonly CreateContainerResponse _container;

    public MinecraftServer(CreateContainerResponse container)
    {
        _container = container;
    }

    public async Task GetContainerLogsAsync()
    {
        Console.WriteLine("Starting log stream...");
        var cancellation = new CancellationTokenSource();

        var logsStream = await _dockerClient.Containers.GetContainerLogsAsync(_container.ID, true,
            new ContainerLogsParameters
            {
                Follow = true,
                ShowStderr = true,
                ShowStdout = true,
            }, cancellation.Token);


        var buffer = new byte[4096];
        while (true)
        {
            var result = await logsStream.ReadOutputAsync(buffer, 0, buffer.Length, default);
            var line = Encoding.UTF8.GetString(buffer, 0, result.Count);
            if (string.IsNullOrEmpty(line)) continue;
            Console.WriteLine(line.TrimEnd());
        }
    }


    public async Task ExecCommandAsync(string command)
    {
        Console.WriteLine($"Executing command: {command}");
        var execConfig = new ContainerExecCreateParameters
        {
            AttachStdin = false,
            AttachStdout = true,
            AttachStderr = true,
            Cmd = new[] {"/bin/sh", "-c", $"echo '{command}' > /fifo"},
            Tty = false
        };

        var execResponse = await _dockerClient.Exec.ExecCreateContainerAsync(_container.ID, execConfig);

        await _dockerClient.Exec.StartContainerExecAsync(execResponse.ID);
    }
}