using System;
using System.Numerics;
using System.Collections.Generic;
using ECC.Tests.Fixtures;
using NUnit.Framework;

namespace ECC.Tests
{
  [TestFixture]
  public class ECCUtilTests
  {

    [Test]
    public void BitLength()
    {
      for (int i = 1; i <= 16; i++)
      {
        Assert.AreEqual(i, ECCUtil.BitLength((int)Math.Pow(2, i)));
      }

      Assert.AreEqual(7, ECCUtil.BitLength(91));
      Assert.AreEqual(14, ECCUtil.BitLength(9119));

      Dictionary<CurveName, int> bitLengths = new Dictionary<CurveName, int>{
        { CurveName.secp128r1, 128 },
        { CurveName.secp160k1, 160 },
        { CurveName.secp160r1, 160 },
        { CurveName.secp192k1, 192 },
        { CurveName.secp192r1, 192 },
        { CurveName.secp256k1, 256 },
        { CurveName.secp256r1, 256 },
      };
      Dictionary<CurveName, int> byteLengths = new Dictionary<CurveName, int>{
        { CurveName.secp128r1, 16 },
        { CurveName.secp160k1, 20 },
        { CurveName.secp160r1, 20 },
        { CurveName.secp192k1, 24 },
        { CurveName.secp192r1, 24 },
        { CurveName.secp256k1, 32 },
        { CurveName.secp256r1, 32 },
      };

      List<CurveName> curveNames = new List<CurveName>(CurveData.data.Keys);
      foreach (CurveName name in curveNames)
      {
        Console.WriteLine(name);
        Curve c = Curve.GetCurveByName(name);
        BigInteger p = c.p;
        int bitLength = ECCUtil.BitLength(p);
        Console.WriteLine(bitLength);
        Assert.AreEqual(bitLengths[name], bitLength);
        Assert.AreEqual(byteLengths[name], (int)Math.Floor((ECCUtil.BitLength(p) + 7f) / 8f));
      }
      
      // Assert.IsTrue(false);
      
      // Assert.AreEqual("115792089237316195423570985008687907853269984665640564039457584007908834671663", p.ToString());
    }

    // [Test]
    // public void Mod() 
    // {

    // }

    // [Test]
    // public void Invert()
    // {

    // }

  }
}