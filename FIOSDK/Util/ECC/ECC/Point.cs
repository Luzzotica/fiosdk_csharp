/* Parts of this software are derivative works of Tom Wu `ec.js` (as part of JSBN).
 * See http://www-cs-students.stanford.edu/~tjw/jsbn/ec.js
 *
 * Copyright (c) 2003-2005  Tom Wu
 * All Rights Reserved.
 *
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to
 * permit persons to whom the Software is furnished to do so, subject to
 * the following conditions:
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS-IS" AND WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS, IMPLIED OR OTHERWISE, INCLUDING WITHOUT LIMITATION, ANY
 * WARRANTY OF MERCHANTABILITY OR FITNESS FOR A PARTICULAR PURPOSE.
 *
 * IN NO EVENT SHALL TOM WU BE LIABLE FOR ANY SPECIAL, INCIDENTAL,
 * INDIRECT OR CONSEQUENTIAL DAMAGES OF ANY KIND, OR ANY DAMAGES WHATSOEVER
 * RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER OR NOT ADVISED OF
 * THE POSSIBILITY OF DAMAGE, AND ON ANY THEORY OF LIABILITY, ARISING OUT
 * OF OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
 *
 * In addition, the following condition applies:
 *
 * All redistributions must retain an intact copy of this copyright notice
 * and disclaimer.
 */

// var assert = require('assert')
// var Buffer = require('safe-buffer').Buffer
// var BigInteger = require('bigi')

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;


namespace ECC
{
  public class Point 
  {
    public Curve curve;
    public BigInteger x, y, z, _zInv;
    public bool compressed = true;

    public BigInteger zInv {
      get { 
        // Mod is strictly positive in these circumstances
        if (_zInv == -1)
        {
          _zInv = ECCUtil.Invert(z, curve.p);
        }

        return _zInv;
      }
    }

    public BigInteger affineX {
      get {
        return ECCUtil.Mod(x * zInv, curve.p);
      }
    }

    public BigInteger affineY {
      get {
        return ECCUtil.Mod(y * zInv, curve.p);
      }
    }

    public Point(Curve curve, BigInteger x, BigInteger y, BigInteger z)
    {
      this.curve = curve;
      this.x = x;
      this.y = y;
      this.z = z;
      this._zInv = new BigInteger(-1);
    }

    public Point Double() 
    {
      // Console.WriteLine("");
      // Console.WriteLine("Double...");
      if (curve.IsInfinity(this)) return this;
      if (y.Sign == 0) return curve.infinity;

      BigInteger x1 = this.x;
      BigInteger y1 = this.y;

      BigInteger y1z1 = ECCUtil.Mod(y1 * z, curve.p);
      BigInteger y1sqz1 = ECCUtil.Mod(y1z1 * y1, curve.p);
      BigInteger a = curve.a;

      // Console.WriteLine("X1: " + x1.ToString());
      // Console.WriteLine("Y1: " + y1.ToString());
      // Console.WriteLine("Y1Z1: " + y1z1.ToString());
      // Console.WriteLine("Y1SQZ1: " + y1sqz1.ToString());

      // w = 3 * x1^2 + a * z1^2
      BigInteger w = 3 * BigInteger.Pow(x1, 2);
      // Console.WriteLine("W Start: " + w.ToString());

      if (a.Sign != 0) {
        w = w + (BigInteger.Pow(z, 2) * a);
        // Console.WriteLine("a.signum: " + a.Sign);
        // Console.WriteLine("W Mid: " + w.ToString());
      }

      w = ECCUtil.Mod(w, this.curve.p);
      // Console.WriteLine("W End: " + w.ToString());
      
      // x3 = 2 * y1 * z1 * (w^2 - 8 * x1 * y1^2 * z1)
      var x3 = ECCUtil.Mod(2 * y1 * z * (BigInteger.Pow(w, 2) - 8 * x1 * BigInteger.Pow(y1, 2) * z), curve.p);
      // y3 = 4 * y1^2 * z1 * (3 * w * x1 - 2 * y1^2 * z1) - w^3
      var y3 = ECCUtil.Mod(4 * BigInteger.Pow(y1, 2) * z * (3 * w * x1 - 2 * BigInteger.Pow(y1, 2) * z) - BigInteger.Pow(w, 3), curve.p);
      // z3 = 8 * (y1 * z1)^3
      var z3 = ECCUtil.Mod(8 * BigInteger.Pow(y1 * z, 3), curve.p);
      // Console.WriteLine("X3: " + x3.ToString());
      // Console.WriteLine("Y3: " + y3.ToString());
      // Console.WriteLine("Z3: " + z3.ToString());

      return new Point(curve, x3, y3, z3);
    }

    public Point Multiply(BigInteger k)
    {
      if (curve.IsInfinity(this)) return this;
      if (k.Sign == 0) return curve.infinity;

      var e = k;
      var h = e * THREE;

      var neg = -this;
      var R = this;

      double bitCount = ECCUtil.BitLength(h);
      BigInteger bitChecker = BigInteger.Pow(2, (int)(bitCount - 2));
      for (var i = bitCount - 2; i > 0; --i) 
      {
        // TODO: Implmenet TestBit, Test bit returns true iff the ith bit is set
        var hBit = (h & bitChecker) == bitChecker;
        var eBit = (e & bitChecker) == bitChecker;
        bitChecker >>= 1;

        R = R.Double();

        if (hBit != eBit) {
          // Console.WriteLine("");
          // Console.WriteLine("Bit mismatch");
          R += (hBit ? this : neg);
        }
      }

      return R;
    }

    public byte[] GetEncoded(bool compressed) 
    {
      if (curve.IsInfinity(this)) return new byte[]{0, 0}; // Infinity point encoded is simply '00'

      BigInteger x = affineX;
      BigInteger y = affineY;
      int byteLength = curve.pByteLength;
      byte[] buffer;

      // 0x02/0x03 | X
      if (compressed) {
        buffer = new byte[1 + (int)byteLength]; //Buffer.allocUnsafe(1 + byteLength)
        buffer[0] = (byte)(y.IsEven ? 0x02 : 0x03);

      // 0x04 | X | Y
      } else {
        buffer = new byte[1 + (int)(byteLength * 2)]; //Buffer.allocUnsafe(1 + byteLength + byteLength)
        buffer[0] = (byte)0x04;

        Array.Copy(y.ToByteArray(), 0, buffer, 1 + (int)byteLength, byteLength);
        //y.toBuffer(byteLength).copy(buffer, 1 + byteLength, buffer)
      }

      //x.toBuffer(byteLength).copy(buffer, 1)
      Array.Copy(x.ToByteArray(), 0, buffer, 1, byteLength);

      return buffer;
    }

    public override String ToString() 
    {
      if (curve.IsInfinity(this)) return "(INFINITY)";

      return $"({affineX.ToString()},{affineY.ToString()})";
    }

    public override bool Equals(object obj)
    {
      // Must be a point
      if (obj.GetType() != typeof(Point)) 
      {
        return false;
      }
      Point other = (Point)obj;

      return x == other.x && y == other.y && z == other.z;
    }

    public override int GetHashCode()
    {
      return (int)(x % int.MaxValue);
    }

    #region Static Functions and Operators

    public static BigInteger THREE = new BigInteger(3);
    
    public static Point FromAffine(Curve c, BigInteger x, BigInteger y)
    {
      return new Point(c, x, y, BigInteger.One);
    }

    public static bool CheckCompression(byte val) 
    {
      return val != 4 || val != 3 || val != 2;
    }

    /// <summary>
    /// Takes a MSB to LSB buffer representing a compressed or uncompressed
    /// point and converts it into one.
    /// If the buffer is LSB to MSB, it will reverse it.
    /// </summary>
    /// <param name="curve">The curve we are acting in</param>
    /// <param name="buffer">The array of bytes</param>
    /// <returns></returns>
    public static Point DecodeFrom(Curve curve, byte[] buffer) 
    {
      bool leftToRight = true;

      // Check the beginning and the end. If the end has the value we are looking
      // for, then we are interpreting from right to left instead.
      if (!CheckCompression(buffer[0])) 
      {
        if (!CheckCompression(buffer[buffer.Length - 1]))
        {
          leftToRight = false;
        }
        else 
        {
          throw new Exception("Error: Buffer must begin with 2, 3, or 4 to determine compression type.");
        }
      }

      // Compression, and byte length
      byte type = buffer[0];
      bool compressed = type != 0x04;
      int byteLength = curve.pByteLength;

      // If we don't have enough bytes to read from (byteLength)
      // We stop, invalid datas
      if (buffer.Length < byteLength + 1) 
      {
        throw new Exception("Invalid sequence length");
      }

      // Create a buffer used to copy data into that has a 0 in front of it
      // This is necessary to avoid negative numbers when working with BigInts
      byte[] buff2 = new byte[byteLength + 1];

      // Copy our data over to the buffer and turn it into our x value
      Array.Copy(buffer, 1, buff2, 1, byteLength);
      // Read left to right, right to left depending on compression found
      BigInteger x = leftToRight ? ECCUtil.BigIntFromBuffer(buff2) : new BigInteger(buff2);

      Point Q;
      if (compressed)
      {
        if (buffer.Length != byteLength + 1)
        {
          throw new Exception("Invalid sequence length");
        }
        else if (type != 0x02 && type != 0x03)
        {
          throw new Exception("Invalid sequence tag");
        }

        var isOdd = type == 0x03;
        Q = curve.PointFromX(isOdd, x);
      } 
      else 
      {
        if (buffer.Length != byteLength * 2 + 1) 
        {
          throw new Exception("Invalid sequence length");
        }
        // assert.equal(buffer.length, 1 + byteLength + byteLength, 'Invalid sequence length')
        Array.Copy(buffer, 1 + byteLength, buff2, 1, byteLength);
        BigInteger y = leftToRight ? ECCUtil.BigIntFromBuffer(buff2) : new BigInteger(buff2);
        // var y = BigInteger.fromBuffer(buffer.slice(1 + byteLength))
        Q = Point.FromAffine(curve, x, y);
      }

      Q.compressed = compressed;
      return Q;
    }

    public static Point operator -(Point a) 
    { 
        return new Point(a.curve, a.x, a.curve.p - a.y, a.z);
    }

    public static Point operator +(Point a, Point b)
    {
      if (a.curve.IsInfinity(a)) return b;
      if (a.curve.IsInfinity(b)) return a;

      var x1 = a.x;
      var y1 = a.y;
      var x2 = b.x;
      var y2 = b.y;

      // BigInteger x3 = ECCUtil.Mod((y2 - y1)^2 / (x2 - x1)^2 - x1 - x2, a.curve.p);
      // BigInteger y3 = ECCUtil.Mod((2 * x1 + x2) * (y2 - y1) / (x2 - x1) - (y2 - y1)^3 / (x2 - x1)^3 - y1, a.curve.p);

      // return Point.FromAffine(a.curve, x3, y3);

      // u = Y2 * Z1 - Y1 * Z2
      BigInteger u = ECCUtil.Mod((y2 * (a.z)) - (y1 * b.z), a.curve.p);
      // v = X2 * Z1 - X1 * Z2
      BigInteger v = ECCUtil.Mod((x2 * (a.z)) - (x1 * b.z), a.curve.p);

      if (v.Sign == 0)
      {
        if (u.Sign == 0)
        {
          return a.Double(); // this == b, so double
        }

        return a.curve.infinity; // this = -b, so infinity
      }
      
      // Cache some values
      var v2 = v * v;
      var v3 = v2 * v;
      var x1v2 = x1 * v2;
      var z1u2 = u * u * a.z;

      // Console.WriteLine("");
      // Console.WriteLine("Add...");
      // Run all our numbers through the function to get the proper X, Y, Z points
      // x3 = v * (z2 * (z1 * u^2 - 2 * x1 * v^2) - v^3)
      var x3 = ECCUtil.Mod(v * (b.z * (z1u2 - (x1v2 << 1)) - v3), a.curve.p);
      // y3 = z2 * (3 * x1 * u * v^2 - y1 * v^3 - z1 * u^3) + u * v^3
      var y3 = ECCUtil.Mod(b.z * (THREE * x1v2 * u - y1 * v3 - z1u2 * u) + u * v3, a.curve.p);
      // z3 = v^3 * z1 * z2
      var z3 = ECCUtil.Mod(v3 * a.z * b.z, a.curve.p);
      // Console.WriteLine("X3: " + x3.ToString());
      // Console.WriteLine("Y3: " + y3.ToString());
      // Console.WriteLine("Z3: " + z3.ToString());

      return new Point(a.curve, x3, y3, z3);
    }

    public static Point operator -(Point a, Point b)
        => a + (-b);

    #endregion
  }
}