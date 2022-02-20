using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using NUnit.Framework;
using ECC;
using ECC.Tests.Fixtures;

namespace ECC.Tests
{
  [TestFixture]
  public class CurveTests
  {
    [SetUp]
    public void Setup()
    {
      // FileStream ostrm;
      // StreamWriter writer;
      // TextWriter oldOut = Console.Out;
      // try
      // {
      //     ostrm = new FileStream ("./Redirect.txt", FileMode.OpenOrCreate, FileAccess.Write);
      //     Console.WriteLine(ostrm.Name);
      //     writer = new StreamWriter (ostrm);
      // }
      // catch (Exception e)
      // {
      //     Console.WriteLine ("Cannot open Redirect.txt for writing");
      //     Console.WriteLine (e.Message);
      //     return;
      // }
      // Console.WriteLine("Setting output to redirect");
      // Console.SetOut(writer);
    }

    [Test]
    public void CreateCurveObject()
    {
      var p = BigInteger.Parse("11", NumberStyles.AllowHexSpecifier);
      var a = BigInteger.Parse("22", NumberStyles.AllowHexSpecifier);
      var b = BigInteger.Parse("33", NumberStyles.AllowHexSpecifier);
      var Gx = BigInteger.Parse("44", NumberStyles.AllowHexSpecifier);
      var Gy = BigInteger.Parse("55", NumberStyles.AllowHexSpecifier);
      var n = BigInteger.Parse("66", NumberStyles.AllowHexSpecifier);
      var h = BigInteger.Parse("77", NumberStyles.AllowHexSpecifier);

      var curve = new Curve(p, a, b, Gx, Gy, n, h);
      Assert.AreEqual(p, curve.p);
      Assert.AreEqual(a, curve.a);
      Assert.AreEqual(b, curve.b);

      Assert.AreEqual(Point.FromAffine(curve, Gx, Gy), curve.G);
      Assert.AreEqual(n, curve.n);
      Assert.AreEqual(h, curve.h);
      Assert.AreEqual(a, curve.a);
      Assert.AreEqual(b, curve.b);
    }

    [Test]
    public void CalculatePublicPoint()
    {
      foreach (CurveTestData data in CurveTestData.data) 
      {
        Curve c = Curve.GetCurveByName(data.QName);
        Console.WriteLine(data.QName);

        BigInteger d = BigInteger.Parse(data.d);
        Console.Write(d.ToString());
        Point Q = c.G.Multiply(d);

        Assert.AreEqual(data.Qx, Q.affineX.ToString());
        Assert.AreEqual(data.Qy, Q.affineY.ToString());
      }
    }

    [Test]
    public void FieldMath()
    {
      // General Elliptic curve formula: y^2 = x^3 + ax + b
      // Testing field: y^2 = x^3 + x (a = 1, b = 0)
      // Wolfram Alpha: solve mod(y^2, 11)=mod(x^3+x, 11)
      // There are 12 valid points on this curve (11 plus point at infinity)
      //   (0,0), (5,8), (7,8), (8,5), (9,10), (10,8)
      //          (5,3), (7,3), (8,6), (9,1),  (10,3)
      //
      // /////////////////////////////////////////////
      // 10                           X
      //  9
      //  8               X     X        X
      //  7
      //  6                        X
      //  5                        X
      //  4
      //  3               X     X        X
      //  2
      //  1                           X
      //  0 X
      //    0 1  2  3  4  5  6  7  8  9 10
      // /////////////////////////////////////////////
      
      BigInteger Gx = BigInteger.Parse("8");
      BigInteger Gy = BigInteger.Parse("6");
      BigInteger n = BigInteger.Parse("12");
      Curve curve = new Curve(BigInteger.Parse("11"), 1, 0, Gx, Gy, n, -1);
      List<Point> points = new List<Point>{
        Point.FromAffine(curve, 0, 0),
        Point.FromAffine(curve, 5, 8), Point.FromAffine(curve, 5, 3),
        Point.FromAffine(curve, 7, 8), Point.FromAffine(curve, 7, 3),
        Point.FromAffine(curve, 8, 5), Point.FromAffine(curve, 8, 6),
        Point.FromAffine(curve, 9, 10), Point.FromAffine(curve, 9, 1),
        Point.FromAffine(curve, 10, 8), Point.FromAffine(curve, 10, 3)
      };

      // pG = P = -P
      var P = curve.G.Multiply(curve.p);
      Assert.AreEqual(P, -curve.G);

      // nG = 0
      var nG = curve.G.Multiply(curve.n);
      Assert.IsTrue(curve.IsInfinity(nG));

      var inf = curve.infinity;
      var a = points[2];
      var b = points[7];
      var z = points[0];
      var y = Point.FromAffine(curve, 1, 1);

      // Validates field elements properly
      Assert.IsTrue(curve.Validate(a));
      Assert.IsTrue(curve.Validate(b));
      Assert.IsTrue(curve.Validate(z));
      Assert.IsTrue(curve.IsOnCurve(z));
      Assert.IsTrue(!curve.IsOnCurve(y));
      Assert.IsTrue(!curve.IsInfinity(a));
      Assert.IsTrue(!curve.IsInfinity(b));
      Assert.IsTrue(curve.IsInfinity(inf));
      Assert.IsTrue(curve.IsOnCurve(inf));

      // Check proper negation
      Assert.AreEqual((-a).ToString(), "(5,8)"); // -(5,3) = (5,8)
      Assert.AreEqual((-b).ToString(), "(9,1)"); // -(9,10) = (9,1)
      // Assert.AreEqual(inf.negate().toString(), '(INFINITY)') // FAILS: can't negate infinity point should fail out gracefully
      Assert.AreEqual((-z).ToString(), "(0,0)"); // -(0,0) = (0,0)

      // Add field elements properly
      Assert.AreEqual((a + b).ToString(), "(9,1)"); // (5,3) + (9,10) = (9,1)
      Assert.AreEqual((b + a).ToString(), "(9,1)"); // (9,10) + (5,3) = (9,1)
      Assert.AreEqual((a + z).ToString(), "(9,10)"); // (5,3) + (0,0) = (9,10)
      Assert.AreEqual((a + y).ToString(), "(8,1)"); // (5,3) + (1,1) = (8,1)  <-- weird result should error out if one of the operands isn't on the curve // FIXME

      Assert.AreEqual((a + inf).ToString(), "(5,3)"); // (5,3) + INFINITY = (5,3)
      Assert.AreEqual((inf + a).ToString(), "(5,3)"); // INFINITY + (5,3) = (5,3)

      // Multiply Field Elements properly
      Assert.AreEqual((a.Multiply(2)).ToString(), "(5,8)"); // (5,3) x 2 = (5,8)
      Assert.AreEqual((a.Multiply(3)).ToString(), "(INFINITY)"); // (5,3) x 3 = INFINITY
      Assert.AreEqual((a.Multiply(4)).ToString(), "(5,3)"); // (5,3) x 4 = (5,3)
      Assert.AreEqual((a.Multiply(5)).ToString(), "(5,8)"); // (5,3) x 5 = (5,8)

      Assert.AreEqual((b.Multiply(2)).ToString(), "(5,8)"); // (9,10) x 2 = (5,8)
      Assert.AreEqual((b.Multiply(3)).ToString(), "(0,0)"); // (9,10) x 3 = (0,0)
      Assert.AreEqual((b.Multiply(4)).ToString(), "(5,3)"); // (9,10) x 4 = (5,3)
      Assert.AreEqual((b.Multiply(5)).ToString(), "(9,1)"); // (9,10) x 5 = (9,1)

      Assert.AreEqual((inf.Multiply(2)).ToString(), "(INFINITY)"); // INFINITY x 2 = INFINITY
      Assert.AreEqual((inf.Multiply(3)).ToString(), "(INFINITY)"); // INFINITY x 3 = INFINITY
      Assert.AreEqual((inf.Multiply(4)).ToString(), "(INFINITY)"); // INFINITY x 4 = INFINITY
      Assert.AreEqual((inf.Multiply(5)).ToString(), "(INFINITY)"); // INFINITY x 5 = INFINITY

      Assert.AreEqual((z.Multiply(2)).ToString(), "(INFINITY)"); // (0,0) x 2 = INFINITY
      Assert.AreEqual((z.Multiply(3)).ToString(), "(0,0)"); // (0,0) x 3 = (0,0)
      Assert.AreEqual((z.Multiply(4)).ToString(), "(INFINITY)"); // (0,0) x 4 = INFINITY
      Assert.AreEqual((z.Multiply(5)).ToString(), "(0,0)"); // (0,0) x 5 = (0,0)

      Assert.AreEqual((a.Multiply(2)).ToString(), a.Double().ToString()); //(  * (2) == .twice()
      Assert.AreEqual((b.Multiply(2)).ToString(), b.Double().ToString());
      Assert.AreEqual((inf.Multiply(2)).ToString(), inf.Double().ToString());
      Assert.AreEqual((z.Multiply(2)).ToString(), z.Double().ToString());

      Assert.AreEqual((a.Multiply(2)).ToString(), (a + a).ToString()); // thi(s * (2) == this.add(this)
      Assert.AreEqual((b.Multiply(2)).ToString(), (b + b).ToString());
      Assert.AreEqual((inf.Multiply(2)).ToString(), (inf + inf).ToString());
      Assert.AreEqual((z.Multiply(2)).ToString(), (z + z).ToString());
    }

    [Test]
    public void IsOnCurve()
    {
      // Ensure each point is on the curve
      foreach (ValidPointData validPoint in ValidPointData.data)
      {
        Curve c = Curve.GetCurveByName(validPoint.curve);
        Point point = Point.FromAffine(c, BigInteger.Parse(validPoint.x), BigInteger.Parse(validPoint.y));

        Assert.IsTrue(c.IsOnCurve(point));
      }

      // secp256k1 functions
      Curve curve = Curve.GetCurveByName(CurveName.secp256k1);

      // Should return true for a point on the curve
      var Q = curve.G.Multiply(1);
      Assert.IsTrue(curve.IsOnCurve(Q));

      // Should return false for points not in the finite field
      var P = Point.FromAffine(curve, curve.p + 1, 0);
      Assert.IsTrue(!curve.IsOnCurve(P));

      // Should return false for points not on the curve
      P = Point.FromAffine(curve, 1, 1);
      Assert.IsTrue(!curve.IsOnCurve(P));

      // Should return true for points at (0, 0) if they are on the curve
      curve = new Curve(11, 1, 0, 8, 6, 12, -1);

      P = Point.FromAffine(curve, 0, 0);
      Assert.IsTrue(curve.IsOnCurve(P));
    }

    [Test]
    public void Validate()
    {
      // Ensure each point is valid
      foreach (ValidPointData validPoint in ValidPointData.data)
      {
        Curve c = Curve.GetCurveByName(validPoint.curve);
        Point point = Point.FromAffine(c, BigInteger.Parse(validPoint.x), BigInteger.Parse(validPoint.y));

        Assert.IsTrue(c.Validate(point));
      }

      // secp256k1
      Curve curve = Curve.GetCurveByName(CurveName.secp256k1);

      // Should validate P where y^2 == x^3 + ax + b (mod p)
      var Q = curve.G.Multiply(1);
      Assert.IsTrue(curve.Validate(Q));

      // Should not validate P where y^2 != x^3 + ax + b (mod p)
      var P = Point.FromAffine(curve, 1, 1);
      Assert.IsFalse(curve.Validate(P));

      // Should not validate P where P = O
      Assert.IsFalse(curve.Validate(curve.infinity));

      // TODO: should not validate P where nP = O
    }

    [Test]
    public void PointFromX()
    {
      // Ensure each point is valid
      foreach (ValidPointData validPoint in ValidPointData.data)
      {
        Curve c = Curve.GetCurveByName(validPoint.curve);
        BigInteger x = BigInteger.Parse(validPoint.x);
        bool odd = !(BigInteger.Parse(validPoint.y).IsEven);

        // Derivces Y coordinate for the curve currectly
        Point point = c.PointFromX(odd, x);

        Assert.AreEqual(validPoint.x.ToString(), point.affineX.ToString());
        Assert.AreEqual(validPoint.y.ToString(), point.affineY.ToString());
      }
    }
  }
}
