using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.RBSS_CS;

namespace DAL1.RBSS_CS
{
    public interface IDatabase
    {
        public void Insert(SimpleDataObject data);
        public SimpleDataObject[] GetAllDataObjects();
    }
}
