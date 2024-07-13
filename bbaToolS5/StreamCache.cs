using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbaToolS5
{
    internal class StreamCache
    {
        private static Dictionary<Stream, int> Cache = new();

        internal static void AddRef(Stream s)
        {
            if (s == null)
                return;
            if (Cache.ContainsKey(s))
                Cache[s]++;
            else
                Cache[s] = 1;
        }

        internal static void RemoveRef(Stream s)
        {
            if (s == null)
                return;
            Cache[s]--;
            if (Cache[s] <= 0)
            {
                Cache.Remove(s);
                s.Close();
            }
        }
    }
}
