using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ECC;

/** Signs transactions using in-process private keys */
public class CsSignatureProvider : SignatureProvider 
{
  /** map public to private keys */
  public Dictionary<string, string> keys = new Dictionary<string, string>();

  /** public keys */
  public List<string> availableKeys = new List<string>();

  /** @param privateKeys private keys to sign with */
  CsSignatureProvider(List<string> privateKeys)
  {
    PrivateKey pKey;
    foreach (string k in privateKeys)
    {
      pKey = new PrivateKey(k);
      string pub = KeyManager.ConvertLegacyPublicKey(pKey.ToPublic().ToString());
      keys[pub] = k;
      availableKeys.Add(pub);
    }
    // foreach (string k in privateKeys) {
    //     string pub = KeyManager.ConvertLegacyPublicKey(ecc.PrivateKey.fromString(k).toPublic().toString());
    //     this.keys.set(pub, k);
    //     this.availableKeys.push(pub);
    // }
  }

  /** Public keys associated with the private keys that the `SignatureProvider` holds */
  public List<string> GetAvailableKeys() {
    return this.availableKeys;
  }

  /** Sign a transaction */
  public Task<PushTransactionArgs> Sign(SignatureProviderArgs args) {
    List<byte> signBuffer = new List<byte>();

    signBuffer.AddRange(ECCUtil.HexStringToByteArray(args.chainId));
    signBuffer.AddRange(args.serializedTransaction);
    signBuffer.AddRange(args.serializedContextFreeData != null ? 
      HashHelper.Sha256(args.serializedContextFreeData.ToArray()) : new byte[32]);

    List<string> signatures = new List<string>();

    foreach (string key in args.requiredKeys)
    {
      
    }

    return null;
    // const signBuf = Buffer.concat([
    //     new Buffer(chainId, 'hex'),
    //     new Buffer(serializedTransaction),
    //     new Buffer(
    //         serializedContextFreeData ?
    //             hexToUint8Array(ecc.sha256(serializedContextFreeData)) :
    //             new Uint8Array(32)
    //     ),
    // ]);
    // const signatures = requiredKeys.map(
    //     (pub) => ecc.Signature.sign(signBuf, this.keys.get(convertLegacyPublicKey(pub))).toString(),
    // );
    // return { signatures, serializedTransaction, serializedContextFreeData };
  }
}
