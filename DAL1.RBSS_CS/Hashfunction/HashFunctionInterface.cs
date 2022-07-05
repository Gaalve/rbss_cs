using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.RBSS_CS;

namespace DAL1.RBSS_CS.Hashfunction
{
    public interface IHashFunction
    {
        public byte[] Hash(byte[] data);
        public byte[] Hash(string str);
        public byte[] Hash(SimpleDataObject dataObject);
    }
}
