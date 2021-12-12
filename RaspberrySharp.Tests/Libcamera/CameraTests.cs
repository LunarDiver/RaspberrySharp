using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RaspberrySharp.Libcamera;

namespace RaspberrySharp.Tests.Libcamera;

[TestClass]
public class CameraTests
{
    public readonly Camera Camera = new Camera();

    [TestMethod]
    public void TakesValidJpegImage()
    {
        byte[] res = Camera.TakeJpegImage();
        
        Assert.AreNotEqual(0, res.Length);

        Assert.AreEqual(0xff, res[0]);
        Assert.AreEqual(0xd8, res[1]);
        Assert.AreEqual(0xff, res[2]);
    }
}