using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL1.RBSS_CS
{
    public class PrecalculatedHash
    {
        public byte[] Hash { get; set;  }


        public PrecalculatedHash()
        {
            Hash = new byte[] { 0 };
        }

        public PrecalculatedHash(byte[] hash)
        {
            Hash = hash;
        }

        private static byte[] Xor(byte[] b1, byte[] b2)
        {
            byte[] nb = b1.ToArray();

            for (int i = 0; i < b2.Length; i++)
            {
                nb[i] ^= b2[i];
            }

            return nb;
        }

        public PrecalculatedHash Bifunctor(byte[] otherHash)
        {
            byte[] b = Hash.Length >= otherHash.Length ? Xor(Hash, otherHash) : Xor(otherHash, Hash);
            return new PrecalculatedHash(b);
        }

        public PrecalculatedHash Bifunctor(PrecalculatedHash otherHash)
        {
            return Bifunctor(otherHash.Hash);
        }
    }
}
