
using System;
using System.Numerics;

public class Curve
{
  public static Curve Secp256k1()
  {
    BigInteger P = new BigInteger(new byte[32]{
        0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,
        0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,
        0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,
        0xFF,0xFF,0xFF,0xFE,0xFF,0xFF,0xFC,0x2F
    });
    // BitConverter.GetBytes
    BigInteger n = BigInteger.One; //new BigInteger(2^256 - 432420386565659656852420866394968145599);
    BigInteger Gx = BigInteger.Parse("79BE667EF9DCBBAC55A06295CE870B07029BFCDB2DCE28D959F2815B16F81798", System.Globalization.NumberStyles.HexNumber);
    BigInteger Gy = BigInteger.Parse("483ADA7726A3C4655DA4FBFC0E1108A8FD17B448A68554199C47D08FFB10D4B8", System.Globalization.NumberStyles.HexNumber);
    Point G = new Point(Gx, Gy);

    Func<Point, Point> doub = (p1) => {
      BigInteger X1 = p1.x;
      BigInteger Y1 = p1.y;
      BigInteger lam = ECCUtil.Mod(3 * X1^2 * ECCUtil.Invert(2 * Y1, P), P);
      BigInteger X3 = ECCUtil.Mod(lam * lam - 2 * X1, P);
      BigInteger Y3 = ECCUtil.Mod(lam * (X1 - X3) - Y1, P);
      return new Point(X3, Y3);
    };

    Func<Point, Point, Point> add = (p1, p2) => {
      BigInteger X1 = p1.x, Y1 = p1.y, X2 = p2.x, Y2 = p2.y;
      // const [X1, Y1, X2, Y2] = [x, y, b.x, b.y];
      if (X1 == 0 || Y1 == 0) return p2;
      if (X2 == 0 || Y2 == 0) return p1;
      if (X1 == X2 && Y1 == Y2) return doub.Invoke(p1);
      if (X1 == X2 && Y1 == -Y2) return Point.ZERO;
      BigInteger lam = ECCUtil.Mod((Y2 - Y1) * ECCUtil.Invert(X2 - X1, P), P);
      BigInteger X3 = ECCUtil.Mod(lam * lam - X1 - X2, P);
      BigInteger Y3 = ECCUtil.Mod(lam * (X1 - X3) - Y1, P);
      return new Point(X3, Y3);
    };

    return new Curve(P, n, G, doub, add);
  }

  public BigInteger P;
  public BigInteger n;
  public Point G;
  public Func<Point, Point, Point> add;
  public Func<Point, Point> doub;

  public Curve(BigInteger P, 
    BigInteger n, 
    Point G, 
    Func<Point, Point> doub,
    Func<Point, Point, Point> add)
  {
    this.P = P;
    this.n = n;
    this.G = G;
    this.add = add;
    this.doub = doub;
  }

  public Point FromAffine(BigInteger x, BigInteger y)
  {
    return new Point(x, y, this);
  }

  public Point DecodeFrom(BigInteger key)
  {

  }
}