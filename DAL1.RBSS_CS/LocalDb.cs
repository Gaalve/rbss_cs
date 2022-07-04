using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models.RBSS_CS;
using Newtonsoft.Json;
using System.Data.Entity.Migrations;
namespace DAL1.RBSS_CS
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
            Console.WriteLine("Adding element: " + data);
            if (ObjectSet.Find(data.Id) == null)
                ObjectSet.Add(new SimpleDataObjectPDS(data));
            SaveChanges();
        }

        public SimpleDataObject[] GetAllDataObjects()
        {
            Console.WriteLine("Getting all Elements");
            return ObjectSet.Select(s => new SimpleDataObject(s.Id, s.Timestamp, 
                JsonConvert.DeserializeObject(s.JsonData)!)).ToArray();
        }
    }
}
