using System;
using System.Collections.Generic;

public class RawTransaction 
{
  public string expiration = "";
  public int refBlockNum = 0;
  public int refBlockPrefix = 0;
  public int maxNetUsageWords = 0;
  public int maxCpuUsageMs = 0;
  public int delaySec = 0;
  public List<RawAction> contextFreeActions = new List<RawAction>();
}