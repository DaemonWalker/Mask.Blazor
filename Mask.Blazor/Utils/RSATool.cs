using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Mask.Blazor.Utils
{
    static class RSATool
    {
        public static string RSAEncrypt(this string content)
        {
            RSA rsa = CreateRsaFromPublicKey(CommonValue.PublicKey);
            byte[] palinDataBytes = EncodingStrOrByte.GetBytes(content, EncodingStrOrByte.EncodingType.UTF8);
            byte[] encryptDataBytes = rsa.Encrypt(palinDataBytes, RSAEncryptionPadding.Pkcs1);
            return Convert.ToBase64String(encryptDataBytes);

        }

        private static RSA CreateRsaFromPublicKey(string publicKeyString)
        {
            byte[] SeqOID = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
            byte[] x509key;
            byte[] seq = new byte[15];
            int x509size;

            x509key = Convert.FromBase64String(publicKeyString);
            x509size = x509key.Length;

            using (var mem = new MemoryStream(x509key))
            {
                using (var binr = new BinaryReader(mem))
                {
                    byte bt = 0;
                    ushort twobytes = 0;
                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8130)
                        binr.ReadByte();
                    else if (twobytes == 0x8230)
                        binr.ReadInt16();
                    else
                        return null;
                    seq = binr.ReadBytes(15);
                    if (!CompareBytearrays(seq, SeqOID))
                        return null;
                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8103)
                        binr.ReadByte();
                    else if (twobytes == 0x8203)
                        binr.ReadInt16();
                    else
                        return null;
                    bt = binr.ReadByte();
                    if (bt != 0x00)
                        return null;
                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8130)
                        binr.ReadByte();
                    else if (twobytes == 0x8230)
                        binr.ReadInt16();
                    else
                        return null;
                    twobytes = binr.ReadUInt16();
                    byte lowbyte = 0x00;
                    byte highbyte = 0x00;
                    if (twobytes == 0x8102)
                        lowbyte = binr.ReadByte();
                    else if (twobytes == 0x8202)
                    {
                        highbyte = binr.ReadByte();
                        lowbyte = binr.ReadByte();
                    }
                    else
                        return null;
                    byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                    int modsize = BitConverter.ToInt32(modint, 0);
                    int firstbyte = binr.PeekChar();
                    if (firstbyte == 0x00)
                    {
                        binr.ReadByte();
                        modsize -= 1;
                    }
                    byte[] modulus = binr.ReadBytes(modsize);
                    if (binr.ReadByte() != 0x02)
                        return null;
                    int expbytes = (int)binr.ReadByte();
                    byte[] exponent = binr.ReadBytes(expbytes);
                    var rsa = RSA.Create();
                    var rsaKeyInfo = new RSAParameters
                    {
                        Modulus = modulus,
                        Exponent = exponent
                    };
                    rsa.ImportParameters(rsaKeyInfo);
                    return rsa;
                }
            }
        }

        private static bool CompareBytearrays(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;
            int i = 0;
            foreach (byte c in a)
            {
                if (c != b[i])
                    return false;
                i++;
            }
            return true;
        }
        static class EncodingStrOrByte
        {
            /// <summary>
            /// 编码方式
            /// </summary>
            public enum EncodingType { UTF7, UTF8, UTF32, Unicode, BigEndianUnicode, ASCII, GB2312 };
            /// <summary>
            /// 处理指定编码的字符串，转换字节数组
            /// </summary>
            /// <param name="str"></param>
            /// <param name="encodingType"></param>
            /// <returns></returns>
            public static byte[] GetBytes(string str, EncodingType encodingType)
            {
                byte[] bytes = null;
                switch (encodingType)
                {
                    //将要加密的字符串转换为指定编码的字节数组
                    case EncodingType.UTF7:
                        bytes = Encoding.UTF7.GetBytes(str);
                        break;
                    case EncodingType.UTF8:
                        bytes = Encoding.UTF8.GetBytes(str);
                        break;
                    case EncodingType.UTF32:
                        bytes = Encoding.UTF32.GetBytes(str);
                        break;
                    case EncodingType.Unicode:
                        bytes = Encoding.Unicode.GetBytes(str);
                        break;
                    case EncodingType.BigEndianUnicode:
                        bytes = Encoding.BigEndianUnicode.GetBytes(str);
                        break;
                    case EncodingType.ASCII:
                        bytes = Encoding.ASCII.GetBytes(str);
                        break;
                    case EncodingType.GB2312:
                        bytes = Encoding.Default.GetBytes(str);
                        break;
                }
                return bytes;
            }

            /// <summary>
            /// 处理指定编码的字节数组，转换字符串
            /// </summary>
            /// <param name="myByte"></param>
            /// <param name="encodingType"></param>
            /// <returns></returns>
            public static string GetString(byte[] myByte, EncodingType encodingType)
            {
                string str = null;
                switch (encodingType)
                {
                    //将要加密的字符串转换为指定编码的字节数组
                    case EncodingType.UTF7:
                        str = Encoding.UTF7.GetString(myByte);
                        break;
                    case EncodingType.UTF8:
                        str = Encoding.UTF8.GetString(myByte);
                        break;
                    case EncodingType.UTF32:
                        str = Encoding.UTF32.GetString(myByte);
                        break;
                    case EncodingType.Unicode:
                        str = Encoding.Unicode.GetString(myByte);
                        break;
                    case EncodingType.BigEndianUnicode:
                        str = Encoding.BigEndianUnicode.GetString(myByte);
                        break;
                    case EncodingType.ASCII:
                        str = Encoding.ASCII.GetString(myByte);
                        break;
                    case EncodingType.GB2312:
                        str = Encoding.Default.GetString(myByte);
                        break;
                }
                return str;
            }
        }
    }
}
