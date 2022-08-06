using Microsoft.EntityFrameworkCore;
using Models.RBSS_CS;
using Newtonsoft.Json;

namespace DAL1.RBSS_CS.Databse
{
    public sealed class LocalDb : DbContext, IDatabase
    {
        private DbSet<SimpleDataObjectPDS> ObjectSet { get; set; }
        public LocalDb() : base()
        {
            
            ObjectSet = Set<SimpleDataObjectPDS>();
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data Source=Application.db;Cache=Shared");
        }

        public void Insert(SimpleDataObject data)
        {
            var pds = ObjectSet.Find(data.Id);
            if (pds == null)
                ObjectSet.Add(new SimpleDataObjectPDS(data));
            else
            {
                pds.Timestamp = data.Timestamp;
                pds.JsonData = JsonConvert.SerializeObject(data.AdditionalProperties, Formatting.Indented);
                ObjectSet.Update(pds);
            }
            SaveChanges();
        }

        public SimpleDataObject[] GetAllDataObjects()
        {
            return ObjectSet.Select(s => new SimpleDataObject(s.Id, s.Timestamp, 
                JsonConvert.DeserializeObject(s.JsonData)!)).ToArray();
        }
    }
}
