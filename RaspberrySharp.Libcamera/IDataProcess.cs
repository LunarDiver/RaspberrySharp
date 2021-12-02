using System.Diagnostics;

namespace RaspberrySharp.Libcamera;

internal interface IDataProcess
{
    public string Path { get; }
    
    public Process GetDefaultProcess();

    public Process GetDefaultProcess(string args);

    public byte[] GetStandardData();

    public byte[] GetStandardData(string args);
    
    public Task<byte[]> GetStandardDataAsync();

    public Task<byte[]> GetStandardDataAsync(string args);
    
    public Task GetContinuousStandardDataAsync(Action<byte[]> data);

    public Task GetContinuousStandardDataAsync(string args, Action<byte[]> data);
}