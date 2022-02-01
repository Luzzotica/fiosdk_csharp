using System;
using System.Numerics;
using System.Collections.Generic;

public static class Constants 
{

  public static class EndPoints {
    public static string addPublicAddress = "add_pub_address";
    public static string SetFioDomainVisibility = "set_fio_domain_public";
    public static string recordObtData = "record_obt_data";
    public static string registerFioAddress = "register_fio_address";
    public static string registerFioDomain = "register_fio_domain";
    public static string rejectFundsRequest = "reject_funds_request";
    public static string requestNewFunds = "new_funds_request";
    public static string transferTokensKey = "transfer_tokens_pub_key";
    public static string transferTokensFioAddress = "transfer_tokens_fio_address";
  }

  public static List<string> feeNoAddressOperation = new List<string>(){
    EndPoints.registerFioDomain,
    EndPoints.registerFioAddress,
    EndPoints.transferTokensKey,
    EndPoints.transferTokensFioAddress
  };

  public static List<string> rawAbiAccountName = new List<string>(){
    "fio.address",
    "fio.reqobt",
    "fio.token",
    "eosio",
    "fio.fee",
    "eosio.msig",
    "fio.treasury",
    "fio.tpid"
  };

  public static int multiplier = 1000000000;

  public static string defaultAccount = "fio.address";

  public static int publicKeyDataSize = 33;
  public static int privateKeyDataSize = 32;
  public static int signatureDataSize = 65;


}