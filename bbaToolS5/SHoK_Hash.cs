using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbaToolS5
{
    internal class SHoK_Hash
    {
        internal static UInt32 GetHash(string s)
        {
            if (s.Length <= 0)
                return 1;
            UInt32 num = 0;
            foreach (char c in s)
            {
                num = c + 16 * num;
                if ((num & 0xF0000000) != 0)
                    num ^= num & 0xF0000000 ^ ((num & 0xF0000000) >> 24);
            }
            if (num != 0)
                return 1812433253 * (num >> 16) - 1989869568 * num;
            else
                return 1;
        }
    }
}
