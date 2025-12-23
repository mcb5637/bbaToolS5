namespace bbaLib
{
    internal static class StreamCache
    {
        private static readonly Dictionary<Stream, int> Cache = [];

        internal static void AddRef(Stream? s)
        {
            if (s == null)
                return;
            if (!Cache.TryAdd(s, 1))
                Cache[s]++;
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
