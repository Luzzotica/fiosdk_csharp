using System;
using System.Numerics;


/// <summary>
/// A Point is an X and a Y coordinate of an elliptic curve, represented by C.
/// All operations: Addition, Multiplication, Inverse, act within the bounds of the curve.
/// </summary>
public class Point 
{
  public static Point ZERO = new Point(BigInteger.Zero, BigInteger.Zero);

  public Curve c = null;
  public BigInteger x;
  public BigInteger y;

  public Point(BigInteger x, BigInteger y, Curve c = null)
  {
    this.x = x;
    this.y = y;

    // If we weren't given a curve, use Secp256K1
    if (c == null) 
    {
      c = Curve.Secp256k1();
    }
  }

  Point Double() 
  {
    // BigInteger X1 = this.x;
    // BigInteger Y1 = this.y;
    // BigInteger lam = Mod(3 * X1^2 * Invert(2 * Y1, c.P), c.P);
    // BigInteger X3 = Mod(lam * lam - 2 * X1, c.P);
    // BigInteger Y3 = Mod(lam * (X1 - X3) - Y1, c.P);
    // return new Point(X3, Y3);
    return c.doub.Invoke(this);
  }

  // Adds point to other point. http://hyperelliptic.org/EFD/g1p/auto-shortw.html
  public Point Add(Point other) 
  {
    return c.add.Invoke(this, other);
    // BigInteger X1 = x, Y1 = y, X2 = other.x, Y2 = other.y;
    // // const [X1, Y1, X2, Y2] = [x, y, b.x, b.y];
    // if (X1 == 0 || Y1 == 0) return other;
    // if (X2 == 0 || Y2 == 0) return this;
    // if (X1 == X2 && Y1 == Y2) return this.Double();
    // if (X1 == X2 && Y1 == -Y2) return Point.ZERO;
    // BigInteger lam = Mod((Y2 - Y1) * Invert(X2 - X1, c.P), c.P);
    // BigInteger X3 = Mod(lam * lam - X1 - X2, c.P);
    // BigInteger Y3 = Mod(lam * (X1 - X3) - Y1, c.P);
    // return new Point(X3, Y3);
  }

  Point Multiply(BigInteger n)
  {
    Point p = Point.ZERO;
    Point d = this;
    while (n > 0) 
    {
      if ((n & 1) == 1) 
      {
        p = p.Add(d);
      }
      d = d.Double();
      n >>= 1;
    }
    return p;
  }
}