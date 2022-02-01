using Cryptography.ECDSA;
using Godot;
using System;
using System.Numerics;

public class PublicKey
{
  String keyString;
  BigInteger keyBigInt;
  // This is the compressed public key
  byte[] keyBuffer;

  public PublicKey(byte[] b) 
  {
    keyBuffer = b;
  }

  public byte[] ToBuffer(bool compressed) 
  {
    if (compressed) 
    {
      return keyBuffer;
    }

    return Secp256K1Manager.PublicKeyDecompress(keyBuffer);
  }

  /** @todo rename to toStringLegacy
    * @arg {string} [pubkey_prefix = 'FIO'] - public key prefix
  */
  public override string ToString() {
    return "FIO" + KeyUtils.CheckEncode(keyBuffer);
  }

  public PublicKey ToUncompressed() {
      var buf = Q.getEncoded(false);
      var point = ecurve.Point.decodeFrom(secp256k1, buf);
      return PublicKey.fromPoint(point);
  }
}
