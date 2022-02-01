// import { Abi, PushTransactionArgs } from './chain-rpc-interfaces';
using System.Collections.Generic;
using System.Threading.Tasks;

/** Arguments to `getRequiredKeys` */
public class AuthorityProviderArgs {
    /** Transaction that needs to be signed */
    public RawTransaction transaction;

    /** Public keys associated with the private keys that the `SignatureProvider` holds */
    List<string> availableKeys;
}

/** Get subset of `availableKeys` needed to meet authorities in `transaction` */
public interface AuthorityProvider {
    /** Get subset of `availableKeys` needed to meet authorities in `transaction` */
    Task<List<string>> GetRequiredKeys(AuthorityProviderArgs args);
}

/** Retrieves raw ABIs for a specified accountName */
public interface AbiProvider {
    /** Retrieve the BinaryAbi */
    Task<BinaryAbi> GetRawAbi(string accountName);
}

/** Structure for the raw form of ABIs */
public class BinaryAbi {
    /** account which has deployed the ABI */
    string accountName;

    /** abi in binary form */
    List<byte> abi;
}

/** Holds a fetched abi */
public class CachedAbi {
    /** abi in binary form */
    List<byte> rawAbi;

    /** abi in structured form */
    Abi abi;
}

/** Arguments to `sign` */
public class SignatureProviderArgs {
    /** Chain transaction is for */
    public string chainId;

    /** Public keys associated with the private keys needed to sign the transaction */
    public List<string> requiredKeys;

    /** Transaction to sign */
    public List<byte> serializedTransaction;

    /** Context-free data to sign */
    public List<byte> serializedContextFreeData;

    /** ABIs for all contracts with actions included in `serializedTransaction` */
    List<BinaryAbi> abis;
}

/** Signs transactions */
public interface SignatureProvider {
    /** Public keys associated with the private keys that the `SignatureProvider` holds */
    List<string> GetAvailableKeys();

    /** Sign a transaction */
    Task<PushTransactionArgs> Sign(SignatureProviderArgs args);
}
