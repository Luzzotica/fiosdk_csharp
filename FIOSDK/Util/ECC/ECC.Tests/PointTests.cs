using System;
using System.Numerics;
using ECC.Tests.Fixtures;
using NUnit.Framework;

namespace ECC.Tests
{
  [TestFixture]
  public class PointTests
  {
    [Test]
    public void Multiply()
    {
      // Make sure multiplication works
      foreach (ValidPointData validPoint in ValidPointData.data)
      {
        Curve c = Curve.GetCurveByName(validPoint.curve);
        BigInteger d = BigInteger.Parse(validPoint.d);
        Point Q = c.G.Multiply(d);
        
        Assert.AreEqual(validPoint.x, Q.affineX.ToString());
        Assert.AreEqual(validPoint.y, Q.affineY.ToString());
      }
    }

    [Test]
    public void DecodeFrom()
    {
      // Ensure each point is on the curve
      foreach (ValidPointData validPoint in ValidPointData.data)
      {
        Curve c = Curve.GetCurveByName(validPoint.curve);
        byte[] buffer = ECCUtil.HexStringToByteArray(validPoint.hex);
        Console.WriteLine(validPoint.curve);
        Console.WriteLine("Buffer Length: " + buffer.Length);

        Point Q = Point.DecodeFrom(c, buffer);
        
        Assert.AreEqual(validPoint.x, Q.affineX.ToString());
        Assert.AreEqual(validPoint.y, Q.affineY.ToString());
        Assert.AreEqual(validPoint.compressed, Q.compressed);
      }

      // Ensure each point is on the curve
      foreach (InvalidPointData invPoint in InvalidPointData.data)
      {
        Curve c = Curve.GetCurveByName(CurveName.secp256k1);
        byte[] buffer = ECCUtil.HexStringToByteArray(invPoint.hex);
        
        var ex = Assert.Throws<Exception>(() => Point.DecodeFrom(c, buffer));
        Assert.That(ex.Message, Is.EqualTo(invPoint.exception));
      }
    }
  }

}