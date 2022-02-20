using System.Collections.Generic;
using System.Numerics;
using NUnit.Framework;
using ECC;

namespace ECC.Tests
{
  [TestFixture]
  public class CurveNameTests
  {
    [SetUp]
    public void Setup()
    {
      
    }

    [Test]
    public void GetCurveByNameReturnsCorrectCurve()
    {
      // Check each implemented curve
      List<CurveName> implementedCurves = new List<CurveName>(CurveData.data.Keys); //new List<CurveName>{ CurveName.secp128r1, CurveName.secp160k1, CurveName.secp160r1, CurveName.secp256k1 };
      foreach (CurveName name in implementedCurves) 
      {
        Curve c = Curve.GetCurveByName(name);
        CurveData curveData = CurveData.data[name];

        Assert.AreEqual(curveData.p, ECCUtil.ToHexString(c.p));
        Assert.AreEqual(curveData.a, ECCUtil.ToHexString(c.a));
        Assert.AreEqual(curveData.b, ECCUtil.ToHexString(c.b));
        Assert.AreEqual(curveData.Gx, ECCUtil.ToHexString(c.G.affineX));
        Assert.AreEqual(curveData.Gy, ECCUtil.ToHexString(c.G.affineY));
        Assert.AreEqual(curveData.n, ECCUtil.ToHexString(c.n));
        Assert.AreEqual(curveData.h, ECCUtil.ToHexString(c.h));

        if (name == CurveName.secp256k1) {
          Assert.AreEqual("115792089237316195423570985008687907853269984665640564039457584007908834671663", c.p.ToString());
        }
      }
    }

    [Test]
    public void CreatePointFromX()
    {
      Assert.AreEqual(Curve.GetCurveByName(CurveName.unknown), null);
    }
  }
}
