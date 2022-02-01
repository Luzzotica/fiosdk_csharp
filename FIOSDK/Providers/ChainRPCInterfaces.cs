// copyright defined in fiojs/LICENSE.txt


using System.Collections.Generic;
/** Structured format for abis */
public class Abi {
    public struct AbiType 
    {
      public string name;
      public string type;
    }

    public struct AbiStruct
    {
      public string name;
      public string b;
      public List<AbiType> fields;
    }

    public struct AbiAction 
    {
      public string name;
      public string type;
      public string ricardianContract;
    }

    public struct AbiTable 
    {
      public string name;
      public string type;
      public string indexType;
      public List<string> keyNames;
      public List<string> keyTypes;
    }

    public struct RicardianClause
    {
      public string id;
      public string body;
    }

    public struct ErrorMessage 
    {
      public string errorCode;
      public string errorMessage;
    }

    public struct AbiExtension 
    {
      public int tag;
      public string value;
    }

    public struct AbiVariant 
    {
      public string name;
      public List<string> types;
    }

    public string version;
    public List<AbiType> types;
    public List<AbiStruct> structs;
    public List<AbiAction> actions;
    public List<AbiTable> tables;
    public List<RicardianClause> ricardianClauses;
    public List<ErrorMessage> errorMessages;
    public List<AbiExtension> abiExtensions;
    public List<AbiVariant> variants;
}

/** Return value of `/v1/chain/get_abi` */
public class GetAbiResult 
{
    public string accountName;
    public Abi abi;
}

/** Subset of `GetBlockResult` needed to calculate TAPoS fields in transactions */
public class BlockTaposInfo {
    public string timestamp;
    public int blockNum;
    public int refBlockPrefix;
}

/** Return value of `/v1/chain/get_block` */
public class GetBlockResult {
    public string timestamp;
    public string producer;
    public int confirmed;
    public string previous;
    public string transactionMroot;
    public string actionMroot;
    public int scheduleVersion;
    public string producerSignature;
    public string id;
    public int blockNum;
    public int refBlockPrefix;
}

/** Return value of `/v1/chain/get_code` */
// export interface GetCodeResult {
//     account_name: string;
//     code_hash: string;
//     wast: string;
//     wasm: string;
//     abi: Abi;
// }

// /** Return value of `/v1/chain/get_info` */
// export interface GetInfoResult {
//     server_version: string;
//     chain_id: string;
//     head_block_num: number;
//     last_irreversible_block_num: number;
//     last_irreversible_block_id: string;
//     head_block_id: string;
//     head_block_time: string;
//     head_block_producer: string;
//     virtual_block_cpu_limit: number;
//     virtual_block_net_limit: number;
//     block_cpu_limit: number;
//     block_net_limit: number;
// }

// /** Return value of `/v1/chain/get_raw_code_and_abi` */
// export interface GetRawCodeAndAbiResult {
//     account_name: string;
//     wasm: string;
//     abi: string;
// }

// /** Return value of `/v1/chain/get_raw_abi` */
// export interface GetRawAbiResult {
//     account_name: string;
//     code_hash: string;
//     abi_hash: string;
//     abi: string;
// }

/** Arguments for `push_transaction` */
public class PushTransactionArgs {
    List<string> signatures;
    List<byte> serializedTransaction;
    List<byte> serializedContextFreeData;
}
