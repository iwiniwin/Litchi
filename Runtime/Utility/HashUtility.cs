using System;

namespace Litchi
{
    public class HashUtility
    {
        public static ulong FNV_offset_basis = 0xcbf29ce484222325;
        public static ulong FNV_prime = 1099511628211; //240 + 28 + 0xb3

        public static ulong FNV(string str)
        {
            if (str == null) return 0;

            ulong hashCode = FNV_offset_basis;
            for (int i = 0; i < str.Length; ++i)
            {
                char ch = Char.ToLowerInvariant(str[i]);

                if (str[i] == '\\')
                {
                    ch = '/';
                }

                hashCode = hashCode * FNV_prime;
                hashCode = hashCode ^ ch;
            }

            return hashCode;
        }
    }
}