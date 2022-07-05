using Models.RBSS_CS;

namespace DAL1.RBSS_CS.Databse
{
    public interface IDatabase
    {
        public void Insert(SimpleDataObject data);
        public SimpleDataObject[] GetAllDataObjects();
    }
}
