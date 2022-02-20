using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using Cryptography.ECDSA;
using ECC;

public class PrivateKey
{
  Curve secp256k1 = Curve.GetCurveByName(CurveName.secp256k1);
  String keyString;
  BigInteger keyBigInt;
  PublicKey publicKey;

  public PrivateKey(BigInteger d) 
  {
    keyBigInt = d;
  }

  public PrivateKey(string s) 
  {
  //   assert.equal(typeof privateStr, 'string', 'privateStr')
  // const match = privateStr.match(/^PVT_([A-Za-z0-9]+)_([A-Za-z0-9]+)$/)

  // if(match === null) {
  //   // legacy WIF - checksum includes the version
  //   const versionKey = keyUtils.checkDecode(privateStr, 'sha256x2')
  //   const version = versionKey.readUInt8(0);
  //   assert.equal(0x80, version, `Expected version ${0x80}, instead got ${version}`)
  //   const privateKey = PrivateKey.fromBuffer(versionKey.slice(1))
  //   const keyType = 'K1'
  //   const format = 'WIF'
  //   return {privateKey, format, keyType}
  // }

  // assert(match.length === 3, 'Expecting private key like: PVT_K1_base58privateKey..')
  // const [, keyType, keyString] = match
  // assert.equal(keyType, 'K1', 'K1 private key expected')
  // const privateKey = PrivateKey.fromBuffer(keyUtils.checkDecode(keyString, keyType))
  // return {privateKey, format: 'PVT', keyType}
  }

  public override string ToString() 
  {
    return ToWif();
  }

  public string ToWif() 
  {
    List<byte> privateKey = new List<byte>();

    // checksum includes the version
    privateKey.Add(0x80);
    privateKey.AddRange(ToBuffer());// Bufferconcat([new Buffer([0x80]), private_key]);
    
    return KeyUtils.CheckEncode(privateKey.ToArray(), "sha256x2");
  }

  /// <summary>
  /// Gets the public key from this private key.
  /// </summary>
  /// <returns></returns>
  public PublicKey ToPublic() 
  {
    if (publicKey != null) 
    {
        // cache
        // S L O W ever`where
        return publicKey;
    }
    publicKey = new PublicKey(secp256k1.G.Multiply(keyBigInt));
    return publicKey;
  }

  public byte[] ToBuffer() 
  {
    return keyBigInt.ToByteArray();
  }

  /**
    ECIES
    @arg {string|Object} pubkey wif, PublicKey object
    @return {Buffer} 64 byte shared secret
  */
  /// <summary>
  /// Takes the public key, and using DH, creates a shared key using it
  /// and this private key.
  /// </summary>
  /// <param name="pubKey"></param>
  /// <returns></returns>
  public byte[] GetSharedSecret(PublicKey pubKey) 
  {
    byte[] KB = pubKey.ToBuffer(false);
    Point KBP = Point.FromAffine(
      secp256k1,
      new BigInteger(KB.Skip(1).Take(32).ToArray<byte>()), // x
      new BigInteger(KB.Skip(33).Take(32).ToArray<byte>()) // y
    );
    Point P = KBP.Multiply(keyBigInt);
    byte[] S = P.x.ToByteArray();
    
    // SHA512 used in ECIES
    return HashHelper.Sha512(S);
  }
}
