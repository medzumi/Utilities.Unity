using System;

namespace ApplicationScripts.Ecs.Utility
{
    public static class StringHasher
    {
        private static string[] _hashtable = new string[0];
        
        public static int ToHash(this string hashingString)
        {
            var defaultHash = hashingString.GetHashCode();
            return GetActualHash(hashingString, defaultHash);
        }

        public static string Dehash(this int hash)
        {
            return _hashtable[hash];
        }

        //ToDo : optimize
        private static int GetActualHash(string hashingString, int hashCode)
        {
            if (hashCode >= _hashtable.Length)
            {
                Array.Resize(ref _hashtable, hashCode);
                _hashtable[hashCode] = hashingString;
                return hashCode;
            }
            else
            {
                if (_hashtable[hashCode] == hashingString)
                {
                    hashCode += 128;
                    return GetActualHash(hashingString, hashCode);
                }
                else
                {
                    return hashCode;
                }
            }
        }
    }
}