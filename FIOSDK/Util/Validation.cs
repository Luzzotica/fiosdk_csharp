using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public static class Validation 
{
  public struct RuleSet 
  {
    bool required;
    int lengthMin;
    int lengthMax;
    
  }

  public class ValidationResult 
  {
    bool isValid = true;
    List<ErrorObj> errors = new List<ErrorObj>();
  }

  public struct ErrorObj 
  {
    string field;
    string message;
  }

  public static ValidationResult validate()
  {
    ValidationResult res = new ValidationResult();
    return res;
  }
}