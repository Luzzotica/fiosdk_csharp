
using System;
using System.Security.Cryptography;

public static class HashHelper 
{
  private static RIPEMD160 ripe = null;
  public static byte[] Ripemd160(byte[] data) 
  {
    if (ripe == null) 
    {
      ripe = RIPEMD160Managed.Create();
    }
    return ripe.ComputeHash(data);
  }

  private static SHA256 sha256 = null;
  public static byte[] Sha256(byte[] data) 
  {
    if (sha256 == null) 
    {
      sha256 = SHA256.Create();
    }
    return sha256.ComputeHash(data);
  }

  private static SHA512 sha512 = null;
  public static byte[] Sha512(byte[] data) 
  {
    if (sha512 == null) 
    {
      sha512 = SHA512.Create();
    }
    return sha512.ComputeHash(data);
  }
}
