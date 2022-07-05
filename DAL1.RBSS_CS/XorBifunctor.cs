using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL1.RBSS_CS
{
    public class XorBifunctor : IBifunctor
    {
        public byte[] Hash { get; set; }

        public XorBifunctor()
        {
            Hash = new byte[]{ 0 };
        }

        private void Xor(byte[] b1, byte[] b2)
        {
            Hash = b1.ToArray();

            for (int i = 0; i < b2.Length; i++)
            {
                Hash[i] ^= b2[i];
            }
        }
        public void Apply(byte[] bytes)
        {
            if (Hash.Length >= bytes.Length) Xor(Hash, bytes);
            else Xor(bytes, Hash);
        }

        public void Apply(IBifunctor other)
        {
            Apply(other.Hash);
        }

        public IBifunctor GetNewEmpty()
        {
            return new XorBifunctor();
        }
    }
}
