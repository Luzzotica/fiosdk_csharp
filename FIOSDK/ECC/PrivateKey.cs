using Godot;
using System;
using System.Collections.Generic;
using System.Numerics;
using Cryptography.ECDSA;

public class PrivateKey
{
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
    byte[] pubKey = Secp256K1Manager.GetPublicKey(ToBuffer(), false);
    publicKey = new PublicKey(pubKey);
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
  public List<byte> getSharedSecret(PublicKey pubKey) 
  {
      
      byte[] KB = pubKey.ToBuffer(false);
      new BigInteger(KB);
      Point KBP = Point.fromAffine(
        secp256k1,
        BigInteger.fromBuffer( KB.slice( 1,33 )), // x
        BigInteger.fromBuffer( KB.slice( 33,65 )) // y
      )
      // byte[] r = ToBuffer();
      Point P = KBP.multiply(keyBigInt);
      byte[] S = P.x.ToBuffer({size: 32})
      // SHA512 used in ECIES
      return HashHelper.Sha512(S);
  }
}
