using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbaToolS5
{
    internal class SHoK_Crypt
    {
        private static readonly UInt32[] Key = [0x298F599D, 0x67AD005D, 0x2AF91C8D, 0x66433D6D];
        private static readonly UInt32 Delta = 0x61C88647;

        private static UInt32 Feistel(UInt32 s, UInt32 v0, UInt32 v1, UInt32 n, UInt32[] k)
        {
            return (((s ^ v0) + (v1 ^ k[((s >> 2) & 3) ^ n & 3])) ^ ((16 * v1 ^ (v0 >> 3)) + ((v1 >> 5) ^ 4 * v0)));
        }

        internal static void Decrypt(byte[] data)
        {
            if (data.Length % 4 > 0)
                throw new IOException("decrypt wrong data length");
            UInt32[] buffer = new UInt32[data.Length / 4];
            Buffer.BlockCopy(data, 0, buffer, 0, data.Length);
            Decrypt(buffer);
            Buffer.BlockCopy(buffer, 0, data, 0, data.Length);
        }
        internal static void Encrypt(byte[] data)
        {
            if (data.Length % 4 > 0)
                throw new IOException("encrypt wrong data length");
            UInt32[] buffer = new UInt32[data.Length / 4];
            Buffer.BlockCopy(data, 0, buffer, 0, data.Length);
            Encrypt(buffer);
            Buffer.BlockCopy(buffer, 0, data, 0, data.Length);
        }

        internal static void Decrypt(UInt32[] data)
        {
            if (data.Length < 2)
                throw new IOException("decrypt data too short");
            DecryptData(data, (uint)data.Length, Key);
        }

        internal static void Encrypt(UInt32[] data)
        {
            if (data.Length < 2)
                throw new IOException("encrypt data too short");
            EncryptData(data, (uint)data.Length, Key);
        }

        private static void DecryptData(UInt32[] data, UInt32 blocks, UInt32[] key)
        {
            UInt32 rounds = 52 / blocks + 6;
            UInt32 sum = (uint)(-Delta * rounds);
            UInt32 last = data[0];
            for (; rounds > 0; rounds--)
            {
                for (UInt32 pos = blocks - 1; pos > 0; pos--)
                {
                    data[pos] -= Feistel(sum, last, data[pos - 1], pos, key);
                    last = data[pos];
                }
                data[0] -= Feistel(sum, last, data[blocks - 1], 0, key);
                last = data[0];
                sum += Delta;
            }
        }

        private static void EncryptData(UInt32[] data, UInt32 blocks, UInt32[] key)
        {
            UInt32 sum = 0;
            UInt32 last = data[blocks - 1];

            for (UInt32 rounds = 52 / blocks + 6; rounds > 0; rounds--)
            {
                sum -= Delta;
                for (UInt32 pos = 0; pos < blocks - 1; pos++)
                {
                    data[pos] += Feistel(sum, data[pos + 1], last, pos, key);
                    last = data[pos];
                }
                data[blocks - 1] += Feistel(sum, data[0], last, blocks - 1, key);
                last = data[blocks - 1];
            }
        }
    }
}
