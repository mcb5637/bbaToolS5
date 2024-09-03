using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbaLib
{
    internal class StreamCache
    {
        private static readonly Dictionary<Stream, int> Cache = [];

        internal static void AddRef(Stream? s)
        {
            if (s == null)
                return;
            if (Cache.ContainsKey(s))
                Cache[s]++;
            else
                Cache[s] = 1;
        }

        internal static void RemoveRef(Stream? s)
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
