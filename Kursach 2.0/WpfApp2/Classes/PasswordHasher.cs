using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2.Classes
{
    internal static class PasswordHasher
    {
        public static ulong HashFunc(string str)
        {
            int key = 11;
            int count = 9;
            ulong size = (ulong)Math.Pow(10, count);
            ulong hash_code, t_hash = 0;
            for (int i = 0; i < str.Length; i++)
            {
                t_hash += (ulong)Math.Pow(key, i) * (ulong)str[i];
                t_hash %= size;
            }
            hash_code = t_hash % size;
            return hash_code;
        }
    }
}
