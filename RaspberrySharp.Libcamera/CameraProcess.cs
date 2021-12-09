using System.Diagnostics;
using System.Text;

namespace RaspberrySharp.Libcamera;

internal class CameraProcess : Process, IDataProcess, IDisposable
{
    /// <inheritdoc />
    public string Path { get; } = "libcamera-still";

    public bool AllowIncompleteData { get; set; }

    public new BinaryReader StandardOutput { get; }

    protected const string ProcessPath = "libcamera-still";

    private readonly ProcessStartInfo _startInfo = new ProcessStartInfo
    {
        FileName = ProcessPath,
        CreateNoWindow = true,
        ErrorDialog = false,
        RedirectStandardOutput = true
    };

    public CameraProcess()
    {
        StartInfo = _startInfo;
        StandardOutput = new BinaryReader(base.StandardOutput.BaseStream, Encoding.Default, true);
    }

    public CameraProcess(string args)
    {
        StartInfo = _startInfo;
        StartInfo.Arguments = args;
        StandardOutput = new BinaryReader(base.StandardOutput.BaseStream, Encoding.Default, true);
    }

    /// <inheritdoc />
    public byte[] GetStandardData()
    {
        if(!HasExited && !AllowIncompleteData)
            throw new InvalidOperationException("Process did not exit yet. Data may be incomplete.");

        byte[] data = StandardOutput.ReadBytes((int)StandardOutput.BaseStream.Length);

        if(StandardOutput.BaseStream.CanSeek)
            StandardOutput.BaseStream.Seek(0, SeekOrigin.Begin);

        return data;
    }

    /// <inheritdoc />
    public async Task GetContinuousStandardDataAsync(Action<byte[]> data, CancellationToken cancellation)
    {
        if(!AllowIncompleteData)
            throw new InvalidOperationException($"You must enable {nameof(AllowIncompleteData)} in order to read continuous data.");

        long lastLength = 0;
        do
        {
            long currLength = StandardOutput.BaseStream.Length;
            if (lastLength != currLength)
            {
                byte[] newData = StandardOutput.ReadBytes((int)(currLength - lastLength));
                data(newData);
                lastLength = StandardOutput.BaseStream.Length;
            }

            // ReSharper disable once MethodSupportsCancellation
            await Task.Delay(100);
        } while (!cancellation.IsCancellationRequested && !HasExited);
    }

    public new void Dispose()
    {
        base.Dispose();
        StandardOutput.Dispose();
    }
}