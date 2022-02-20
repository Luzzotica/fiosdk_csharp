// var assert = require('assert')
// var BigInteger = require('bigi')

// var Point = require('./point')
using System;
using System.Globalization;
using System.Numerics;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

namespace ECC
{
  public class Curve 
  {
    public static Dictionary<CurveName, Curve> cachedCurves = new Dictionary<CurveName, Curve>();
    public static Curve GetCurveByName(CurveName name) 
    {
      // if (cachedCurves.ContainsKey(name)) {
      //   return cachedCurves[name];
      // }

      // If we don't have a curve with that name return null
      if (!CurveData.data.ContainsKey(name)) {
        return null;
      }

      CurveData curveData = CurveData.data[name];
      var p = BigInteger.Parse(curveData.p, NumberStyles.AllowHexSpecifier);
      var a = BigInteger.Parse(curveData.a, NumberStyles.AllowHexSpecifier);
      var b = BigInteger.Parse(curveData.b, NumberStyles.AllowHexSpecifier);
      var n = BigInteger.Parse(curveData.n, NumberStyles.AllowHexSpecifier);
      var h = BigInteger.Parse(curveData.h, NumberStyles.AllowHexSpecifier);
      var Gx = BigInteger.Parse(curveData.Gx, NumberStyles.AllowHexSpecifier);
      var Gy = BigInteger.Parse(curveData.Gy, NumberStyles.AllowHexSpecifier);

      // Cache the curve for future use
      Curve c = new Curve(p, a, b, Gx, Gy, n, h);
      cachedCurves[name] = c;

      return c;
    }

    public BigInteger p, a, b, n, h;
    public Point G;
    public Point infinity;
    // Result caching
    public BigInteger pOverFour;
    public int pByteLength;

    public Curve(BigInteger p, 
      BigInteger a, 
      BigInteger b, 
      BigInteger Gx, 
      BigInteger Gy, 
      BigInteger n, 
      BigInteger h) 
    {
      this.p = p;
      this.a = a;
      this.b = b;
      this.G = Point.FromAffine(this, Gx, Gy);
      this.n = n;
      this.h = h;

      this.infinity = new Point(this, -1, -1, BigInteger.Zero);

      // result caching
      this.pOverFour = (p + BigInteger.One) >> 2;

      // determine size of p in bytes
      this.pByteLength = (int)Math.Floor((ECCUtil.BitLength(p) + 7f) / 8f);
      Console.WriteLine("Test: " + this.pByteLength);
    }

    public Point PointFromX(bool isOdd, BigInteger x) {
      BigInteger alpha = ECCUtil.Mod(BigInteger.Pow(x, 3) + (a * x) + b, p);
      BigInteger beta = BigInteger.ModPow(alpha, pOverFour, p);
      // var alpha = x.pow(3).add(this.a.multiply(x)).add(this.b).mod(this.p)
      // var beta = alpha.modPow(this.pOverFour, this.p) // XXX: not compatible with all curves

      var y = beta;
      if (beta.IsEven ^ !isOdd) {
        y = this.p - y; // -y % p
      }

      return Point.FromAffine(this, x, y);
    }

    public bool IsInfinity(Point p)
    {
      if (p == this.infinity) return true;

      return p.z.Sign == 0 && p.y.Sign != 0;
    }

      public bool IsOnCurve(Point Q) 
      {
        if (this.IsInfinity(Q)) return true;

        var x = Q.affineX;
        var y = Q.affineY;
        var a = this.a;
        var b = this.b;
        var p = this.p;

        // Check that xQ and yQ are integers in the interval [0, p - 1]
        if (x.Sign < 0 || x.CompareTo(p) >= 0) return false;
        if (y.Sign < 0 || y.CompareTo(p) >= 0) return false;

        // and check that y^2 = x^3 + ax + b (mod p)
        var lhs = ECCUtil.Mod(y * y, p);
        var rhs = ECCUtil.Mod(BigInteger.Pow(x, 3) + (a * x) + b, p);
        return lhs.Equals(rhs);
        //   var lhs = y.square().mod(p)
        //   var rhs = x.pow(3).add(a.multiply(x)).add(b).mod(p)
        //   return lhs.equals(rhs)
      }

      // /**
      //  * Validate an elliptic curve point.
      //  *
      //  * See SEC 1, section 3.2.2.1: Elliptic Curve Public Key Validation Primitive
      //  */
      public bool Validate(Point Q) 
      {
        // Check Q != O
        if (IsInfinity(Q)) 
        {
          // throw new Exception("Point is at infinity");
          return false;
        }
        else if (!IsOnCurve(Q))
        {
          // throw new Exception("Point is not on the curve");
          return false;
        }
        // assert(!this.isInfinity(Q), 'Point is at infinity')
        // assert(this.isOnCurve(Q), '')

        // Check nQ = O (where Q is a scalar multiple of G)
        var nQ = Q.Multiply(n);
        if (!IsInfinity(nQ))
        {
          return false;
        }
        // assert(this.isInfinity(nQ), 'Point is not a scalar multiple of G')

        return true;
      }
  }

  public enum CurveName
  {
    secp128r1, secp160k1, secp160r1, secp192k1, secp192r1, secp256k1, secp256r1, unknown
  }

  public class CurveData
  {
    public static Dictionary<CurveName, CurveData> data = new Dictionary<CurveName, CurveData>{
      { 
        CurveName.secp128r1, new CurveData {
          p="00fffffffdffffffffffffffffffffffff", 
          a="00fffffffdfffffffffffffffffffffffc", 
          b="00e87579c11079f43dd824993c2cee5ed3",
          n="00fffffffe0000000075a30d1b9038a115",
          h="01",
          Gx="161ff7528b899b2d0c28607ca52c5b86",
          Gy="00cf5ac8395bafeb13c02da292dded7a83" 
        } 
      },
      { 
        CurveName.secp160k1, new CurveData {
          p="00fffffffffffffffffffffffffffffffeffffac73", 
          a="00",
          b="07",
          n="0100000000000000000001b8fa16dfab9aca16b6b3",
          h="01",
          Gx="3b4c382ce37aa192a4019e763036f4f5dd4d7ebb",
          Gy="00938cf935318fdced6bc28286531733c3f03c4fee" 
        } 
      },
      { 
        CurveName.secp160r1, new CurveData {
          p="00ffffffffffffffffffffffffffffffff7fffffff", 
          a="00ffffffffffffffffffffffffffffffff7ffffffc", 
          b="1c97befc54bd7a8b65acf89f81d4d4adc565fa45",
          n="0100000000000000000001f4c8f927aed3ca752257",
          h="01",
          Gx="4a96b5688ef573284664698968c38bb913cbfc82",
          Gy="23a628553168947d59dcc912042351377ac5fb32" 
        }
      },
      { 
        CurveName.secp192k1, new CurveData {
          p="00fffffffffffffffffffffffffffffffffffffffeffffee37", 
          a="00",
          b="03",
          n="00fffffffffffffffffffffffe26f2fc170f69466a74defd8d",
          h="01",
          Gx="00db4ff10ec057e9ae26b07d0280b7f4341da5d1b1eae06c7d",
          Gy="009b2f2f6d9c5628a7844163d015be86344082aa88d95e2f9d" 
        } 
      },
      { 
        CurveName.secp192r1, new CurveData {
          p="00fffffffffffffffffffffffffffffffeffffffffffffffff", 
          a="00fffffffffffffffffffffffffffffffefffffffffffffffc",
          b="64210519e59c80e70fa7e9ab72243049feb8deecc146b9b1",
          n="00ffffffffffffffffffffffff99def836146bc9b1b4d22831",
          h="01",
          Gx="188da80eb03090f67cbf20eb43a18800f4ff0afd82ff1012",
          Gy="07192b95ffc8da78631011ed6b24cdd573f977a11e794811" 
        } 
      },
      { 
        CurveName.secp256k1, new CurveData {
          p="00fffffffffffffffffffffffffffffffffffffffffffffffffffffffefffffc2f", 
          a="00", 
          b="07",
          n="00fffffffffffffffffffffffffffffffebaaedce6af48a03bbfd25e8cd0364141",
          h="01",
          Gx="79be667ef9dcbbac55a06295ce870b07029bfcdb2dce28d959f2815b16f81798",
          Gy="483ada7726a3c4655da4fbfc0e1108a8fd17b448a68554199c47d08ffb10d4b8" 
        } 
      },
      { 
        CurveName.secp256r1, new CurveData {
          p="00ffffffff00000001000000000000000000000000ffffffffffffffffffffffff", 
          a="00ffffffff00000001000000000000000000000000fffffffffffffffffffffffc", 
          b="5ac635d8aa3a93e7b3ebbd55769886bc651d06b0cc53b0f63bce3c3e27d2604b",
          n="00ffffffff00000000ffffffffffffffffbce6faada7179e84f3b9cac2fc632551",
          h="01",
          Gx="6b17d1f2e12c4247f8bce6e563a440f277037d812deb33a0f4a13945d898c296",
          Gy="4fe342e2fe1a7f9b8ee7eb4a7c0f9e162bce33576b315ececbb6406837bf51f5" 
        } 
      },
    };

    public string p = "", a = "", b = "", n = "", h = "", Gx = "", Gy = "";
  }
}


// Curve.prototype.

// Curve.prototype.isInfinity = function (Q) {
//   if (Q === this.infinity) return true

//   return Q.z.signum() === 0 && Q.y.signum() !== 0
// }

// Curve.prototype.isOnCurve = function (Q) {
//   if (this.isInfinity(Q)) return true

//   var x = Q.affineX
//   var y = Q.affineY
//   var a = this.a
//   var b = this.b
//   var p = this.p

//   // Check that xQ and yQ are integers in the interval [0, p - 1]
//   if (x.signum() < 0 || x.compareTo(p) >= 0) return false
//   if (y.signum() < 0 || y.compareTo(p) >= 0) return false

//   // and check that y^2 = x^3 + ax + b (mod p)
//   var lhs = y.square().mod(p)
//   var rhs = x.pow(3).add(a.multiply(x)).add(b).mod(p)
//   return lhs.equals(rhs)
// }

// /**
//  * Validate an elliptic curve point.
//  *
//  * See SEC 1, section 3.2.2.1: Elliptic Curve Public Key Validation Primitive
//  */
// Curve.prototype.validate = function (Q) {
//   // Check Q != O
//   assert(!this.isInfinity(Q), 'Point is at infinity')
//   assert(this.isOnCurve(Q), 'Point is not on the curve')

//   // Check nQ = O (where Q is a scalar multiple of G)
//   var nQ = Q.multiply(this.n)
//   assert(this.isInfinity(nQ), 'Point is not a scalar multiple of G')

//   return true
// }

// module.exports = Curve