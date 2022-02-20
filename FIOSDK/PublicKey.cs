using Cryptography.ECDSA;
using ECC;
using System;
using System.Numerics;
using System.Text.RegularExpressions;

public class PublicKey
{
  Curve c = Curve.GetCurveByName(CurveName.secp256k1);
  String keyString;
  BigInteger keyBigInt;
  // This is the compressed public key
  // byte[] keyBuffer;
  Point keyPoint;

  public PublicKey(byte[] b)
  {
    keyPoint = Point.DecodeFrom(c, b);
  }

  public PublicKey(Point Q)
  {
    keyPoint = Q;
  }

  public PublicKey(string s, string prefix)
  {
    Match m = Regex.Match(s, "^PUB_([A-Za-z0-9]+)_([A-Za-z0-9]+)$");
    
    if (m.Success)
    {

    }
    else
    {
      // Legacy
      // Regex.
    }
  }

  public byte[] ToBuffer() 
  {
    return keyPoint.GetEncoded(keyPoint.compressed);
    // return Secp256K1Manager.PublicKeyDecompress(keyBuffer);
  }

  public byte[] ToBuffer(bool compressed) 
  {
    return keyPoint.GetEncoded(compressed);
    // return Secp256K1Manager.PublicKeyDecompress(keyBuffer);
  }

  /** @todo rename to toStringLegacy
    * @arg {string} [pubkey_prefix = 'FIO'] - public key prefix
  */
  public override string ToString() 
  {
    return "FIO" + KeyUtils.CheckEncode(ToBuffer());
  }

  public PublicKey ToUncompressed() 
  {
    // If we are already uncompressed, then just return ourselves
    if (!keyPoint.compressed) 
    {
      return this;
    }

    // Otherwise decompress
    return new PublicKey(keyPoint.GetEncoded(false));
  }

  // public static bool IsValid(PublicKey key) 
  // {
  //   try 
  //   {

  //   }
  // }

  // public static bool IsValid(PublicKey key) 
  // {
    
  // }
}
