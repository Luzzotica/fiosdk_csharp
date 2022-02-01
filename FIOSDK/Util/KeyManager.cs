using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

public enum KeyType {
    k1, r1
}

public class Key 
{
  public KeyType type;
  public byte[] data;

  public Key(KeyType type, byte[] data) 
  {
    this.type = type;
    this.data = data;
  }
}

public static class KeyManager
{
  

  public static byte[] DigestSuffixRipemd160(byte[] data, string suffix) 
  {
    byte[] d = new byte[data.Length + suffix.Length];
    Array.Copy(data, d, data.Length);
    Array.Copy(Encoding.ASCII.GetBytes(suffix), 0, d, data.Length, suffix.Length);
    return HashHelper.Ripemd160(d);
  }

  public static Key StringToKey(string s, KeyType type, int size, string suffix)
  {
    byte[] whole = NumericHelpers.Base58ToBinary(size + 4, s);
    byte[] resized = new byte[size];
    Array.Copy(whole, resized, size);
    Key result = new Key(type, resized);

    byte[] digest = DigestSuffixRipemd160(result.data, suffix);
    if (digest[0] != whole[size + 0] || digest[1] != whole[size + 1]
      || digest[2] != whole[size + 2] || digest[3] != whole[size + 3]) 
    {
      throw new Exception("checksum doesn\'t match");
    }
    return result;
  }

  public static string KeyToString(Key key, string suffix, string prefix) 
  {
    byte[] digest = DigestSuffixRipemd160(key.data, suffix);
    byte[] whole = new byte[key.data.Length + 4];
    for (int i = 0; i < key.data.Length; ++i) 
    {
        whole[i] = key.data[i];
    }
    for (int i = 0; i < 4; ++i) 
    {
        whole[i + key.data.Length] = digest[i];
    }
    return prefix + NumericHelpers.BinaryToBase58(whole);
  }

  /** Convert key in `s` to binary form */
  public static Key StringToPublicKey(string s) 
  {
    if (s.Substring(0, 3).Equals("FIO")) 
    {
      byte[] keyData = NumericHelpers.Base58ToBinary(Constants.publicKeyDataSize + 4, s.Substring(3));
      Key key = new Key(KeyType.k1, keyData);
      
      // RIPEMD160 ripe = RIPEMD160Managed.Create();
      byte[] digest = HashHelper.Ripemd160(key.data); //ripe.ComputeHash(key.data);
      if (digest[0] != keyData[Constants.publicKeyDataSize] || digest[1] != keyData[34]
        || digest[2] != keyData[35] || digest[3] != keyData[36]) 
      {
        throw new Exception("Checksum doesn\'t match");
      }
      return key;
    } 
    else if (s.Substring(0, 7) == "PUB_K1_") 
    {
      return StringToKey(s.Substring(7), KeyType.k1, Constants.publicKeyDataSize, "K1");
    } 
    else if (s.Substring(0, 7) == "PUB_R1_") 
    {
      return StringToKey(s.Substring(7), KeyType.r1, Constants.publicKeyDataSize, "R1");
    } 
    else 
    {
      throw new Exception("unrecognized public key format");
    }
  }

  /** Convert `key` to string (base-58) form */
  public static string PublicKeyToString(Key key) 
  {
    if (key.type == KeyType.k1 && key.data.Length == Constants.publicKeyDataSize) 
    {
      return KeyToString(key, "K1", "PUB_K1_");
    } 
    else if (key.type == KeyType.r1 && key.data.Length == Constants.publicKeyDataSize) 
    {
      return KeyToString(key, "R1", "PUB_R1_");
    } 
    else 
    {
      throw new Exception("unrecognized public key format");
    }
  }

/** If a key is in the legacy format (`FIO` prefix), then convert it to the new format (`PUB_K1_`).
 * Leaves other formats untouched
 */
  public static string ConvertLegacyPublicKey(string s) 
  {
    if (s.Substring(0, 3).Equals("FIO"))
    {
      return PublicKeyToString(StringToPublicKey(s));
    }
    return s;
  }
}