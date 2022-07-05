using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL1.RBSS_CS.Databse;
using Models.RBSS_CS;

namespace DAL1.RBSS_CS
{
    public class DatabaseStub: IDatabase
    {
        public void Insert(SimpleDataObject data)
        {
            
        }

        public SimpleDataObject[] GetAllDataObjects()
        {
            return Array.Empty<SimpleDataObject>();
        }
    }
}
