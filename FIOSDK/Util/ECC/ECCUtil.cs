using System;
using System.Numerics;

public static class  ECCUtil 
{
  public static BigInteger Mod(BigInteger a, BigInteger b) 
  {
    BigInteger result = a % b;
    return result >= 0 ? result : b + result;
  }

  // Inverses number over modulo
  public static BigInteger Invert(BigInteger number, BigInteger modulo) 
  {
    if (number == 0 || modulo <= 0) {
      throw new Exception($"invert: expected positive integers, got n={number} mod={modulo}");
    }

    // Eucledian GCD https://brilliant.org/wiki/extended-euclidean-algorithm/
    BigInteger a = Mod(number, modulo);
    BigInteger b = modulo;
    BigInteger x = 0, y = 1, u = 1, v = 0; 
    while (a != 0) {
      BigInteger q = b / a;
      BigInteger r = b % a;
      BigInteger m = x - u * q;
      BigInteger n = y - v * q;
      b = a;
      a = r;
      x = u;
      y = v;
      u = m;
      v = n;
    }

    BigInteger gcd = b;
    if (gcd != 1) throw new Exception("Invert: does not exist");
    return Mod(x, modulo);
  }
}
