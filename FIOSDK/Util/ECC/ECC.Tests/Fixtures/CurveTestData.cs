using System.Collections.Generic;

namespace ECC.Tests.Fixtures 
{
  public class CurveTestData
  {
    public static List<CurveTestData> data = new List<CurveTestData>{
      new CurveTestData {
        d = "971761939728640320549601132085879836204587084162",
        QName = CurveName.secp160r1,
        Qx = "466448783855397898016055842232266600516272889280",
        Qy = "1110706324081757720403272427311003102474457754220",
        QHex = "0251b4496fecc406ed0e75a24a3c03206251419dc0"
      },
      new CurveTestData {
        d = "702232148019446860144825009548118511996283736794",
        QName = CurveName.secp160r1,
        Qx = "1176954224688105769566774212902092897866168635793",
        Qy = "1130322298812061698910820170565981471918861336822",
        QHex = ""
      },
      new CurveTestData {
        d = "399525573676508631577122671218044116107572676710",
        QName = CurveName.secp160r1,
        Qx = "420773078745784176406965940076771545932416607676",
        Qy = "221937774842090227911893783570676792435918278531",
        QHex = "0349b41e0e9c0369c2328739d90f63d56707c6e5bc"
      }
    };

    public string d = "";
    public CurveName QName = CurveName.unknown;
    public string Qx = "", Qy = "", QHex = "";
  }
}