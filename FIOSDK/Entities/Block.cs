using System.Collections.Generic;

public class Block 
{
  public string timestamp;
  public string producer;
  public int confirmed;
  public string previous;
  public string transaction_mroot;
  public string action_mroot;
  public int schedule_version;
  public List<Dictionary<string, dynamic>> header_extensions = new List<Dictionary<string, dynamic>>();
  public string producer_signature;
  public List<Dictionary<string, dynamic>> transactions = new List<Dictionary<string, dynamic>>();
  public List<Dictionary<string, dynamic>> block_extensions = new List<Dictionary<string, dynamic>>();
  public string id;
  public int block_num;
  public int ref_block_prefix;
}