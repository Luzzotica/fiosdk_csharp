using Godot;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class Transactions 
{
  public static HttpClient httpClient = new HttpClient();

  string baseUrl;
  Dictionary<string, AbiResponse> abiMap = new Dictionary<string, AbiResponse>();

  string publicKey;
  string privateKey;
  string serializeEndpoint = "chain/serialize_json";

  public string GetActor(string publicKey="") 
  {
    return "";
  }

  /// <summary>
  /// Returned information
  /// {
  ///   server_version: string
  ///   chain_id: string
  ///   head_block_num: integer
  ///   last_irreversible_block_num: integer
  ///   last_irreversible_block_id: string
  ///   head_block_id: string
  ///   head_block_time: string
  ///   head_block_producer: string
  ///   virtual_block_cpu_limit: integer
  ///   virtual_block_net_limit: integer
  ///   block_cpu_limit: integer
  ///   block_net_limit: integer
  ///   server_version_string: string
  /// }
  /// </summary>
  /// <returns></returns>
  public async Task<Chain> GetChainInfo() 
  {
    // Create and send the request to get the chain info
    var request = new HttpRequestMessage
    {
        RequestUri = new Uri($"{baseUrl}chain/get_info"),
        Method = new HttpMethod("GET"),
        Headers =
            {
                { "Accept", "application/json" },
                { "Content-Type", "application/json" }
            }
    };
    HttpResponseMessage response = await Transactions.httpClient.SendAsync(request);
    string responseBody = await response.Content.ReadAsStringAsync();
    Chain chain = JsonConvert.DeserializeObject<Chain>(responseBody);

    // Return the information received
    return chain;
  }

  public async Task<Block> GetBlock(Chain chain) 
  {
    // Check that we have the appropriate info
    if (chain == null) 
    {
      throw new Exception("Chain cannot be null");
    }
    if (chain.last_irreversible_block_num == 0) 
    {
      throw new Exception("Chain's last irreversible block num must have a value");
    }

    // Create and send the request to get the block info
    var request = new HttpRequestMessage
    {
        RequestUri = new Uri($"{baseUrl}chain/get_block"),
        Method = new HttpMethod("POST"),
        Headers =
            {
                { "Accept", "application/json" },
                { "Content-Type", "application/json" }
            },
        Content = new StringContent("{ \"block_num_or_id\": " + chain.last_irreversible_block_num + " }")
    };
    HttpResponseMessage response = await Transactions.httpClient.SendAsync(request);
    string responseBody = await response.Content.ReadAsStringAsync();
    Block block = JsonConvert.DeserializeObject<Block>(responseBody);

    return block;
  }

  public async Task PushToServer(RawTransaction transaction, string endpoint, bool dryRun) 
  {
    // Get the chain and the block, throw exceptions if anything goes wrong.
    Chain chain = new Chain();
    Block block = new Block();
    try 
    {
      chain = await GetChainInfo();
    }
    catch (Exception e)
    {
      GD.Print(e.ToString());
      // TODO: Make this more clear
    }
    try
    {
      block = await GetBlock(chain);
    }
    catch (Exception e) 
    {
      GD.Print(e.ToString());
      // TODO: Make this more clear
    }

    // Track our private keys
    List<string> privateKeys = new List<string>();
    privateKeys.Add(this.privateKey);

    // Prepare the transaction
    transaction.refBlockNum = block.block_num & 0xFFFF;
    transaction.refBlockPrefix = block.ref_block_prefix;
    DateTime expiration = DateTime.Parse(chain.head_block_time);
    expiration.AddSeconds(180);
    transaction.expiration = expiration.ToUniversalTime().ToString("YYYY-MM-DDTHH:mm:ss.sss");

    if (dryRun)
    {

    }
    else 
    {

    }
  }

  public async Task PrepareTransaction(RawTransaction transaction, string chainId, string[] privateKeys, Dictionary<string, dynamic> abiMap) 
  {
    
  }

}