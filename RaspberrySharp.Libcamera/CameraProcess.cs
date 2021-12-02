using System.Diagnostics;

namespace RaspberrySharp.Libcamera;

internal class CameraProcess : IDataProcess
{
    /// <inheritdoc />
    public string Path { get; } = "libcamera-still";

    /// <inheritdoc />
    public Process GetDefaultProcess()
    {
        return GetDefaultProcess(string.Empty);
    }

    /// <inheritdoc />
    public Process GetDefaultProcess(string args)
    {
        return new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = Path,
                Arguments = args,
                CreateNoWindow = true,
                ErrorDialog = false,
                RedirectStandardOutput = true
            }
        };
    }

    /// <inheritdoc />
    public byte[] GetStandardData()
    {
        return GetStandardData(string.Empty);
    }

    /// <inheritdoc />
    public byte[] GetStandardData(string args)
    {
        TimeSpan timeout = TimeSpan.FromSeconds(5);

        Process proc = GetDefaultProcess(args);
        proc.Start();
        bool closed = proc.WaitForExit((int)timeout.TotalMilliseconds);

        if(!closed)
            throw new TimeoutException($"Process did not exit after {timeout.TotalSeconds} seconds.");

        using var stream = new MemoryStream(new byte[proc.StandardOutput.BaseStream.Length]);
        proc.StandardOutput.BaseStream.CopyTo(stream);

        return stream.ToArray();
    }

    /// <inheritdoc />
    public async Task<byte[]> GetStandardDataAsync()
    {
        return await GetStandardDataAsync(string.Empty);
    }

    /// <inheritdoc />
    public async Task<byte[]> GetStandardDataAsync(string args)
    {
        TimeSpan timeout = TimeSpan.FromSeconds(5);

        Process proc = GetDefaultProcess(args);
        proc.Start();
        bool closed = proc.WaitForExit((int)timeout.TotalMilliseconds);

        if(!closed)
            throw new TimeoutException($"Process did not exit after {timeout.TotalSeconds} seconds.");

        await using var stream = new MemoryStream(new byte[proc.StandardOutput.BaseStream.Length]);
        await proc.StandardOutput.BaseStream.CopyToAsync(stream);

        return stream.ToArray();
    }

    /// <inheritdoc />
    public async Task GetContinuousStandardDataAsync(Action<byte[]> data, CancellationToken cancellation)
    {
        await GetContinuousStandardDataAsync(string.Empty, data, cancellation);
    }

    /// <inheritdoc />
    public async Task GetContinuousStandardDataAsync(string args, Action<byte[]> data, CancellationToken cancellation)
    {
        Process proc = GetDefaultProcess(args);
        proc.Start();
        Stream output = proc.StandardOutput.BaseStream;

        long lastLength = 0;
        do
        {
            if(lastLength != output.Length)
            {
                lastLength = output.Length;
                byte[] newData = new byte[output.Length];
                output.Read(newData);
                data(newData);
            }

            // ReSharper disable once MethodSupportsCancellation
            await Task.Delay(100);
        } while(!cancellation.IsCancellationRequested);
    }
}