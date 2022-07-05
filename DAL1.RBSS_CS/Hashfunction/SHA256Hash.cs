using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Models.RBSS_CS;

namespace DAL1.RBSS_CS.Hashfunction
{
    public class SHA256Hash : IHashFunction
    {
        public byte[] Hash(byte[] data)
        {
            return SHA256.HashData(data);
        }

        public byte[] Hash(string str)
        {
            return Hash(Encoding.ASCII.GetBytes(str));
        }

        public byte[] Hash(SimpleDataObject dataObject)
        {
            return Hash(dataObject.ToJson());
        }
    }
}
