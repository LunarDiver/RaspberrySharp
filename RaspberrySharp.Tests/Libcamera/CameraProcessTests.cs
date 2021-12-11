using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RaspberrySharp.Libcamera;

namespace RaspberrySharp.Tests.Libcamera;

[TestClass]
public class CameraProcessTests
{
    [TestMethod]
    public void TestMethod1()
    {
        var test = new Camera();
        test.TakeJpegImage();
    }
}