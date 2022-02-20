using System;
using System.Collections.Generic;

namespace ECC.Tests.Fixtures 
{
  public class InvalidPointData
  {
    public static List<InvalidPointData> data = new List<InvalidPointData>{
      new InvalidPointData{
        description = "Invalid sequence tag",
        exception = "Invalid sequence tag",
        hex = "0179be667ef9dcbbac55a06295ce870b07029bfcdb2dce28d959f2815b16f81798"
      },
      new InvalidPointData{
        description = "Sequence too short",
        exception = "Invalid sequence length",
        hex = "0479be667ef9dcbbac55a06295ce870b07029bfcdb2dce28d959f2815b16f81798483ada7726a3c4655da4fbfc0e1108a8fd17b448a68554199c47d08ffb10"
      },
      new InvalidPointData{
        description = "Sequence too short (compressed)",
        exception = "Invalid sequence length",
        hex = "0279be667ef9dcbbac55a06295ce870b07029bfcdb2dce28d959f2815b16f8"
      },
      new InvalidPointData{
        description = "Sequence too long",
        exception = "Invalid sequence length",
        hex = "0479be667ef9dcbbac55a06295ce870b07029bfcdb2dce28d959f2815b16f81798483ada7726a3c4655da4fbfc0e1108a8fd17b448a68554199c47d08ffb10d4b80000"
      },
      new InvalidPointData{
        description = "Sequence too long (compressed)",
        exception = "Invalid sequence length",
        hex = "0279be667ef9dcbbac55a06295ce870b07029bfcdb2dce28d959f2815b16f817980000"
      },
    };

    public string description = "", exception = "", hex = "";
  }
}