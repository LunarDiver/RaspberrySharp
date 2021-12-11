namespace RaspberrySharp.Libcamera;

public class Camera
{
    public byte[] TakeJpegImage()
    {
        using var proc = new CameraProcess("-t 1 -n -o - --width 640 --height 480");
        proc.Start();
        proc.WaitForExit((int)TimeSpan.FromSeconds(5).TotalMilliseconds);
        return proc.GetStandardData();
    }
}