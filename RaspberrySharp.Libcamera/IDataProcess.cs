using System.Diagnostics;

namespace RaspberrySharp.Libcamera;

internal interface IDataProcess
{
    public string Path { get; }

    public byte[] GetStandardData();

    public Task GetContinuousStandardDataAsync(Action<byte[]> data, CancellationToken cancellation);
}