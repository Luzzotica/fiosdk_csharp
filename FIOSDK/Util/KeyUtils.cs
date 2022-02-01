using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

public static class KeyUtils {
  private static byte[] buffer = new byte[4];

  /**
    @arg {Buffer} keyBuffer data
    @arg {string} keyType = sha256x2, K1, etc
    @return {string} checksum encoded base58 string
  */
  /// <summary>
  /// Encodes the given bytes of a key in the WIF manner.
  /// </summary>
  /// <param name="keyBuffer">A key, private or public.</param>
  /// <param name="keyType">The type of key provided.</param>
  /// <returns></returns>
  public static string CheckEncode(byte[] keyBuffer, string keyType = null) 
  {
    List<byte> b = new List<byte>();
    if (keyType == "sha256x2") 
    {
      b.AddRange(keyBuffer);
      Array.Copy(HashHelper.Sha256(HashHelper.Sha256(keyBuffer)), buffer, 4);
      b.AddRange(buffer);
      // const checksum = hash.sha256(hash.sha256(keyBuffer)).slice(0, 4);

      // Base58 encode our data
      return NumericHelpers.BinaryToBase58(b.ToArray());
    } 
    else 
    {
      b.AddRange(keyBuffer);
      // const check = [keyBuffer];
      if (keyType != null) 
      {
        b.AddRange(Encoding.UTF8.GetBytes(keyType));
          // check.push(Buffer.from(keyType));
      }
      Array.Copy(HashHelper.Ripemd160(b.ToArray()), buffer, 4);
      b.Clear();
      b.AddRange(keyBuffer);
      b.AddRange(buffer);

      // const checksum = HashHelper.Ripemd160(b.ToArray()).slice(0, 4);
      return NumericHelpers.BinaryToBase58(b.ToArray());
    }
  }

  /**
    @arg {Buffer} keyString data
    @arg {string} keyType = sha256x2, K1, etc
    @return {string} checksum encoded base58 string
  */
  // function checkDecode(keyString, keyType = null) 
  // {
  //   assert(keyString != null, 'private key expected')
  //   const buffer = new Buffer(base58.decode(keyString))
  //   const checksum = buffer.slice(-4)
  //   const key = buffer.slice(0, -4)

  //   let newCheck
  //   if(keyType === 'sha256x2') { // legacy
  //     newCheck = hash.sha256(hash.sha256(key)).slice(0, 4) // WIF (legacy)
  //   } else {
  //     const check = [key]
  //     if(keyType) {
  //       check.push(Buffer.from(keyType))
  //     }
  //     newCheck = hash.ripemd160(Buffer.concat(check)).slice(0, 4) //PVT
  //   }

  //   if (checksum.toString() !== newCheck.toString()) {
  //     throw new Error('Invalid checksum, ' +
  //       `${checksum.toString('hex')} != ${newCheck.toString('hex')}`
  //     )
  //   }

  //   return key
  // }
}
