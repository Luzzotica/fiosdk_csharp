using System;
using System.Numerics;

namespace ECC
{
  public static class  ECCUtil 
  {
    public static int BitLength(BigInteger a) {
      // This solves the issue with rounding
      double value = BigInteger.Log(a, 2.0);
      int ret = (int)value;
      if (ret == value) 
      {
        return ret;
      }
      else 
      {
        return ret + 1;
      }
    }

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

    public static byte[] HexStringToByteArray(String hex)
    {
      int numChars = hex.Length;

      // If we don't have an even number of characters, we have a problem
      if (numChars % 2 == 1) {
        throw new Exception("Expected even number of characters in hex string");
      }

      // byte[] bytes = new byte[numChars / 2];
      // for (int i = numChars; i > 0; i -= 2)
      //   bytes[(numChars - i) / 2] = Convert.ToByte(hex.Substring(i - 2, 2), 16);
      // return bytes;
      byte[] bytes = new byte[numChars / 2];
      for (int i = 0; i < numChars; i += 2)
        bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
      return bytes;
    }

    public static string ToHexString(BigInteger b)
    {
      string hex = b.ToString("x");
      if (hex.Length % 2 == 1) {
        return "0" + hex;
      }
      return hex;
    }

    /// <summary>
    /// BigInteger in C# interprets from Least Significant Bit to Most Significant Bit
    /// which is backwards of most.
    /// This is a helper method to reverse the direction of a buffer and
    /// create a BigInteger out of it.
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public static BigInteger BigIntFromBuffer(byte[] buffer) 
    {
      return new BigInteger(buffer.Reverse().ToArray());
    }

  }

  

}
