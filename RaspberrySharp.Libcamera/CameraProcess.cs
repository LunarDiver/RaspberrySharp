using System.Diagnostics;
using System.Text;

namespace RaspberrySharp.Libcamera;

internal class CameraProcess : Process, IDataProcess
{
    /// <inheritdoc />
    public string Path { get; } = "libcamera-still";

    public bool AllowIncompleteData { get; set; }

    public new Stream StandardOutput { get; }

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
        StandardOutput = base.StandardOutput.BaseStream;
    }

    public CameraProcess(string args)
    {
        StartInfo = _startInfo;
        StartInfo.Arguments = args;
        StandardOutput = base.StandardOutput.BaseStream;
    }

    /// <inheritdoc />
    public byte[] GetStandardData()
    {
        if(!HasExited && !AllowIncompleteData)
            throw new InvalidOperationException("Process did not exit yet. Data may be incomplete.");

        using var memory = new MemoryStream();
        StandardOutput.CopyTo(memory);

        return memory.ToArray();
    }

    /// <inheritdoc />
    public async Task GetContinuousStandardDataAsync(Action<byte> data, CancellationToken cancellation)
    {
        if(!AllowIncompleteData)
            throw new InvalidOperationException($"You must enable {nameof(AllowIncompleteData)} in order to read continuous data.");

        //long lastLength = 0;
        int newData;
        do
        {
            newData = StandardOutput.ReadByte();

            if(newData != -1)
                data((byte)newData);

            //long currLength = StandardOutput.Value.BaseStream.Length;
            //if (lastLength != currLength)
            //{
            //    byte[] newData = StandardOutput.Value.ReadBytes((int)(currLength - lastLength));
            //    data(newData);
            //    lastLength = StandardOutput.Value.BaseStream.Length;
            //}

            // ReSharper disable once MethodSupportsCancellation
            await Task.Delay(1);
        } while (!cancellation.IsCancellationRequested && newData != -1);
    }
}