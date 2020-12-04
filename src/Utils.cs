using System.Text;

namespace DMRNG
{
    public class Utils
    {
        /**
         * This the Jenkins one-at-a-time hash function
         * https://en.wikipedia.org/wiki/Jenkins_hash_function#one_at_a_time
         * It's just here as a cheap convenience for the command line program --
         * feel free to provide your own string to int function, if you need it,
         * when integrating DMRNG with your own code.
         */
        public static int Hash(string s)
        {
            int i = 0;
            byte[] key = Encoding.Unicode.GetBytes(s);
            int length = key.Length;
            uint hash = 0;
            while (i != length) {
                hash += key[i++];
                hash += hash << 10;
                hash ^= hash >> 6;
            }
            hash += hash << 3;
            hash ^= hash >> 11;
            hash += hash << 15;
            return (int)hash;
        }
    }
}
