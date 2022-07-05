using Models.RBSS_CS;

namespace DAL1.RBSS_CS.Databse
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
