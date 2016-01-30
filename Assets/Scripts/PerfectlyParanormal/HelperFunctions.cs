using UnityEngine;
using System.Collections;
using System;

namespace PerfectlyParanormal
{

    public static class HelperFunctions
    {

        public static Hashtable Hash(params object[] args)
        {
            Hashtable hashTable = new Hashtable(args.Length / 2);
            if (args.Length % 2 != 0)
            {
                Debug.LogError("Error: Hash requires an even number of arguments!");
                return null;
            }
            else
            {
                int i = 0;
                while (i < args.Length - 1)
                {
                    hashTable.Add(args[i], args[i + 1]);
                    i += 2;
                }
                return hashTable;
            }
        }

        public static bool TryGetKey<T>(this Hashtable table, string key, out T v, T def) 
        {
            if (table.ContainsKey(key))
            {
                v = (T)table[key];
                return true;
            }
            else v = def;
            
            return false;
        }
    }

}