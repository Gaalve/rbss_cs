using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.RBSS_CS;
using Newtonsoft.Json;

namespace DAL1.RBSS_CS
{
    public class SimpleDataObjectPDS
    {
        [Key]
        public string Id { get; set; }
        public long Timestamp { get; set; }
        public string JsonData { get; set; }

        public SimpleDataObjectPDS(string id, long timestamp, string jsonData)
        {
            Id = id;
            Timestamp = timestamp;
            JsonData = jsonData;
        }

        public SimpleDataObjectPDS(SimpleDataObject sdo)
        {
            Id = sdo.Id;
            Timestamp = sdo.Timestamp;
            JsonData = JsonConvert.SerializeObject(sdo.AdditionalProperties, Formatting.Indented);
        }

        public SimpleDataObjectPDS(SimpleObjectWrapper sow)
        {
            Id = sow.Data.Id;
            Timestamp = sow.Data.Timestamp;
            JsonData = JsonConvert.SerializeObject(sow.Data.AdditionalProperties, Formatting.Indented);
        }
    }
}
