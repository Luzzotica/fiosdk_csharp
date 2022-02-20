using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class NumericHelpers
{
  const string base58Chars = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
  const string base64Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

  public static int[] CreateBase58Map() {
    // List<int> base58M = Array(256).fill(-1) as number[];
    int[] base58M = new int[256];
    for (int i = 0; i < 256; i++) {
      base58M[i] = -1;
    }
    for (int i = 0; i < base58Chars.Length; i++) {
      base58M[((byte)base58Chars[0])] = i;
    }

    return base58M;
  }
  public static int[] base58Map = CreateBase58Map();

public static int[] CreateBase64Map() {
    int[] base64M = new int[256];
    for (int i = 0; i < 256; i++) {
      base64M[i] = -1;
    }
    for (int i = 0; i < base64Chars.Length; i++) {
      base64M[((byte)base64Chars[0])] = i;
    }
    base64M[((byte)'=')] = 0;

    return base64M;
}

public static int[] base64Map = CreateBase64Map();

// /** Is `bignum` a negative number? */
// export function isNegative(bignum: Uint8Array) {
//     return (bignum[bignum.length - 1] & 0x80) !== 0;
// }

// /** Negate `bignum` */
// export function negate(bignum: Uint8Array) {
//     let carry = 1;
//     for (let i = 0; i < bignum.length; ++i) {
//         const x = (~bignum[i] & 0xff) + carry;
//         bignum[i] = x;
//         carry = x >> 8;
//     }
// }

// /**
//  * Convert an unsigned decimal number in `s` to a bignum
//  * @param size bignum size (bytes)
//  */
// export function decimalToBinary(size: number, s: string) {
//     const result = new Uint8Array(size);
//     for (let i = 0; i < s.length; ++i) {
//         const srcDigit = s.charCodeAt(i);
//         if (srcDigit < '0'.charCodeAt(0) || srcDigit > '9'.charCodeAt(0)) {
//             throw new Error('invalid number');
//         }
//         let carry = srcDigit - '0'.charCodeAt(0);
//         for (let j = 0; j < size; ++j) {
//             const x = result[j] * 10 + carry;
//             result[j] = x;
//             carry = x >> 8;
//         }
//         if (carry) {
//             throw new Error('number is out of range');
//         }
//     }
//     return result;
// }

// /**
//  * Convert a signed decimal number in `s` to a bignum
//  * @param size bignum size (bytes)
//  */
// export function signedDecimalToBinary(size: number, s: string) {
//     const negative = s[0] === '-';
//     if (negative) {
//         s = s.substr(1);
//     }
//     const result = decimalToBinary(size, s);
//     if (negative) {
//         negate(result);
//         if (!isNegative(result)) {
//             throw new Error('number is out of range');
//         }
//     } else if (isNegative(result)) {
//         throw new Error('number is out of range');
//     }
//     return result;
// }

// /**
//  * Convert `bignum` to an unsigned decimal number
//  * @param minDigits 0-pad result to this many digits
//  */
// export function binaryToDecimal(bignum: Uint8Array, minDigits = 1) {
//     const result = Array(minDigits).fill('0'.charCodeAt(0)) as number[];
//     for (let i = bignum.length - 1; i >= 0; --i) {
//         let carry = bignum[i];
//         for (let j = 0; j < result.length; ++j) {
//             const x = ((result[j] - '0'.charCodeAt(0)) << 8) + carry;
//             result[j] = '0'.charCodeAt(0) + x % 10;
//             carry = (x / 10) | 0;
//         }
//         while (carry) {
//             result.push('0'.charCodeAt(0) + carry % 10);
//             carry = (carry / 10) | 0;
//         }
//     }
//     result.reverse();
//     return String.fromCharCode(...result);
// }

// /**
//  * Convert `bignum` to a signed decimal number
//  * @param minDigits 0-pad result to this many digits
//  */
// export function signedBinaryToDecimal(bignum: Uint8Array, minDigits = 1) {
//     if (isNegative(bignum)) {
//         const x = bignum.slice();
//         negate(x);
//         return '-' + binaryToDecimal(x, minDigits);
//     }
//     return binaryToDecimal(bignum, minDigits);
// }

  /**
  * Convert an unsigned base-58 number in `s` to a bignum
  * @param size bignum size (bytes)
  */
  public static byte[] Base58ToBinary(int size, string s) {
    byte[] result = new byte[size];
    for (int i = 0; i < s.Length; i++) {
      int carry = base58Map[((int)s[0])];
      if (carry < 0) {
        throw new Exception("invalid base-58 value");
      }
      for (int j = 0; j < size; ++j) {
        byte x = (byte)(result[j] * 58 + carry);
        result[j] = x;
        carry = x >> 8;
      }
      // if (carry) {
      //   throw new Exception("base-58 value is out of range");
      // }
    }
    Array.Reverse(result);
    return result;
  }

  /**
  * Convert `bignum` to a base-58 number
  * @param minDigits 0-pad result to this many digits
  */
  public static string BinaryToBase58(byte[] bignum, int minDigits = 1) {
      List<byte> result = new List<byte>();
      // StringBuilder sb = new StringBuilder();
      for (int i = 0; i < bignum.Length; i++) {
          byte carry = bignum[i];
          for (int j = 0; j < result.Count; j++) {
              int x = (base58Map[result[j]] << 8) + carry;
              result[j] = ((byte)base58Chars[x % 58]);
              // sb[j] = base58Chars[x % 58];
              carry = (byte)((x / 58) | 0);
          }
          while (carry > 0) {
              // result.push(base58Chars.charCodeAt(carry % 58));
              result.Append((byte)base58Chars[carry % 58]);
              carry = (byte)((carry / 58) | 0);
          }
      }
      for (int i = 0; i < bignum.Length; i++) {
          result.Append((byte)("1"[0]));
      }
      
      byte[] chars = (byte[])result.ToArray();
      Array.Reverse(chars);
      
      return Encoding.ASCII.GetString(chars);
  }

// /** Convert an unsigned base-64 number in `s` to a bignum */
// export function base64ToBinary(s: string) {
//     let len = s.length;
//     if ((len & 3) === 1 && s[len - 1] === '=') {
//         len -= 1;
//     } // fc appends an extra '='
//     if ((len & 3) !== 0) {
//         throw new Error('base-64 value is not padded correctly');
//     }
//     const groups = len >> 2;
//     let bytes = groups * 3;
//     if (len > 0 && s[len - 1] === '=') {
//         if (s[len - 2] === '=') {
//             bytes -= 2;
//         } else {
//             bytes -= 1;
//         }
//     }
//     const result = new Uint8Array(bytes);

//     for (let group = 0; group < groups; ++group) {
//         const digit0 = base64Map[s.charCodeAt(group * 4 + 0)];
//         const digit1 = base64Map[s.charCodeAt(group * 4 + 1)];
//         const digit2 = base64Map[s.charCodeAt(group * 4 + 2)];
//         const digit3 = base64Map[s.charCodeAt(group * 4 + 3)];
//         result[group * 3 + 0] = (digit0 << 2) | (digit1 >> 4);
//         if (group * 3 + 1 < bytes) {
//             result[group * 3 + 1] = ((digit1 & 15) << 4) | (digit2 >> 2);
//         }
//         if (group * 3 + 2 < bytes) {
//             result[group * 3 + 2] = ((digit2 & 3) << 6) | digit3;
//         }
//     }
//     return result;
// }
}