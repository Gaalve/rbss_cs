using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.RBSS_CS;

namespace DAL1.RBSS_CS.Hashfunction
{
    public class StableHash : IHashFunction
    {
        public byte[] Hash(byte[] data)
        {
            unchecked
            {
                int hash1 = (5381 << 16) + 5381;
                int hash2 = hash1;

                for (int i = 0; i < data.Length; i += 2)
                {
                    hash1 = ((hash1 << 5) + hash1) ^ data[i];
                    if (i == data.Length - 1)
                        break;
                    hash2 = ((hash2 << 5) + hash2) ^ data[i + 1];
                }

                return BitConverter.GetBytes(hash1 + (hash2 * 1566083941));
            }
        }

        public byte[] Hash(string str)
        {
            return Hash(str.ToCharArray().Select(c => (byte)c).ToArray());
        }

        public byte[] Hash(SimpleDataObject dataObject)
        {
            var str = dataObject.toStringWithoutTime();
            return Hash(str);
        }
    }
}
